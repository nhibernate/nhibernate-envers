using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tools;
using NHibernate.Event;
using NHibernate.Proxy;

namespace NHibernate.Envers.Reader
{
	public partial class AuditReader : IAuditReaderImplementor
	{
		private readonly AuditConfiguration verCfg;
		private readonly ICrossTypeRevisionChangesReader _crossTypeRevisionChangesReader;

		public AuditReader(AuditConfiguration verCfg, ISession session,
								  ISessionImplementor sessionImplementor)
		{
			this.verCfg = verCfg;
			SessionImplementor = sessionImplementor;
			Session = session;
			_crossTypeRevisionChangesReader = new CrossTypeRevisionChangesReader(this, verCfg);
			FirstLevelCache = new FirstLevelCache();
		}

		public ISessionImplementor SessionImplementor { get; }
		public ISession Session { get; }
		public FirstLevelCache FirstLevelCache { get; }

		public T Find<T>(object primaryKey, long revision)
		{
			return (T)Find(typeof(T), primaryKey, revision);
		}

		public object Find(System.Type cls, object primaryKey, long revision)
		{
			return Find(cls.FullName, primaryKey, revision);
		}

		public object Find(string entityName, object primaryKey, long revision)
		{
			return Find(entityName, primaryKey, revision, false);
		}

		public object Find(string entityName, object primaryKey, long revision, bool includeDeletions)
		{
			ArgumentsTools.CheckNotNull(primaryKey, "Primary key");
			ArgumentsTools.CheckPositive(revision, "Entity revision");

			if (!verCfg.EntCfg.IsVersioned(entityName))
			{
				throw new NotAuditedException(entityName, entityName + " is not versioned!");
			}

			object result;
			if (FirstLevelCache.TryGetValue(entityName, revision, primaryKey, out result))
			{
				return result;
			}

			// The result is put into the cache by the entity instantiator called from the query
			result = CreateQuery().ForEntitiesAtRevision(entityName, revision, includeDeletions)
				.Add(AuditEntity.Id().Eq(primaryKey)).GetSingleResult();

			return result;
		}

		public IEnumerable<long> GetRevisions(System.Type cls, object primaryKey)
		{
			var entityName = cls.FullName;
			return GetRevisions(entityName, primaryKey);
		}

		public IEnumerable<long> GetRevisions(string entityName, object primaryKey)
		{
			ArgumentsTools.CheckNotNull(primaryKey, "Primary key");

			if (!verCfg.EntCfg.IsVersioned(entityName))
			{
				throw new NotAuditedException(entityName, entityName + " is not versioned!");
			}

			var resultList = CreateQuery().ForRevisionsOfEntity(entityName, false, true)
				.AddProjection(AuditEntity.RevisionNumber())
				.AddOrder(AuditEntity.RevisionNumber().Asc())
				.Add(AuditEntity.Id().Eq(primaryKey))
				.GetResultList();
			return from object revision in resultList select Convert.ToInt64(revision);
		}

		public DateTime GetRevisionDate(long revision)
		{
			ArgumentsTools.CheckPositive(revision, "Entity revision");

			var query = verCfg.RevisionInfoQueryCreator.RevisionDateQuery(Session, revision);

			var timestampObject = query.UniqueResult();
			if (timestampObject == null)
			{
				throw new RevisionDoesNotExistException(revision);
			}

			// The timestamp object is either a date or a long
			return timestampObject is DateTime ? (DateTime)timestampObject : new DateTime((long)timestampObject);
		}

		public long GetRevisionNumberForDate(DateTime date)
		{
			var query = verCfg.RevisionInfoQueryCreator.RevisionNumberForDateQuery(Session, date);

			var res = query.UniqueResult();
			if (res == null)
			{
				throw new RevisionDoesNotExistException(date);
			}

			return Convert.ToInt64(res);
		}


		public object FindRevision(long revision)
		{
			ArgumentsTools.CheckPositive(revision, "revision");

			var revisions = new List<long>(1) { revision };
			var query = verCfg.RevisionInfoQueryCreator.RevisionsQuery(Session, revisions);

			var revisionData = query.UniqueResult();

			if (revisionData == null)
			{
				throw new RevisionDoesNotExistException(revision);
			}

			return revisionData;
		}

		public T FindRevision<T>(long revision)
		{
			return (T)FindRevision(revision);
		}

		public IDictionary<long, object> FindRevisions(IEnumerable<long> revisions)
		{
			var res = new Dictionary<long, object>();
			fillRevisionsResult(res, revisions);
			return res;
		}

		public IDictionary<long, T> FindRevisions<T>(IEnumerable<long> revisions)
		{
			var res = new Dictionary<long, T>();
			fillRevisionsResult(res, revisions);
			return res;
		}

		
		private void fillRevisionsResult<T>(IDictionary<long, T> result, IEnumerable<long> revisions)
		{
			foreach (var revision in revisions)
			{
				ArgumentsTools.CheckPositive(revision, "Entity revision");
			}

			var revisionList = verCfg.RevisionInfoQueryCreator.RevisionsQuery(Session, revisions).List();
			foreach (T revision in revisionList)
			{
				var rev = verCfg.RevisionInfoNumberReader.RevisionNumber(revision);
				result[rev] = revision;
			}
		}

		public object GetCurrentRevision(bool persist)
		{
			if (!(Session is IEventSource sessionAsEventSource))
			{
				throw new NotSupportedException("The provided session is not an EventSource!");
			}

			// Obtaining the current audit sync
			var auditSync = verCfg.AuditProcessManager.Get(sessionAsEventSource);

			// And getting the current revision data
			return auditSync.CurrentRevisionData(Session, persist);
		}

		public T GetCurrentRevision<T>(bool persist)
		{
			return (T)GetCurrentRevision(persist);
		}

		public AuditQueryCreator CreateQuery()
		{
			return new AuditQueryCreator(verCfg, this);
		}

		public ICrossTypeRevisionChangesReader CrossTypeRevisionChangesReader()
		{
			if (!verCfg.GlobalCfg.IsTrackEntitiesChangedInRevisionEnabled)
			{
				throw new AuditException("This API is designed for Envers default mechanism of tracking entities modified in a given revision."
				                         +
				                         " Extend DefaultTrackingModifiedEntitiesRevisionEntity, utilize ModifiedEntityNamesAttribute annotation or set "
				                         + "'nhibernate.envers.track_entities_changed_in_revision' parameter to true.");
			}
			return _crossTypeRevisionChangesReader;
		}

		public string GetEntityName(object primaryKey, long revision, object entity)
		{
			if (entity is INHibernateProxy proxy)
			{
				entity = proxy.HibernateLazyInitializer.GetImplementation();
			}
			if (FirstLevelCache.TryGetEntityName(primaryKey, revision, entity, out var ret))
			{
				return ret;
			}
			throw new HibernateException(
				"Envers can't resolve entityName for historic entity. The id, revision and entity is not on envers first level cache.");
		}

		public bool IsEntityClassAudited(System.Type entityClass)
		{
			return IsEntityNameAudited(entityClass.FullName);
		}

		public bool IsEntityNameAudited(string entityName)
		{
			return verCfg.EntCfg.IsVersioned(entityName);
		}
	}
}
