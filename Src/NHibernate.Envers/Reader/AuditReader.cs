using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Query;
using NHibernate.Envers.Query.Criteria;
using NHibernate.Envers.Tools;
using NHibernate.Event;
using NHibernate.Util;

namespace NHibernate.Envers.Reader
{
	public class AuditReader : IAuditReaderImplementor
	{
		public AuditReader(AuditConfiguration verCfg, ISession session,
								  ISessionImplementor sessionImplementor)
		{
			this.verCfg = verCfg;
			SessionImplementor = sessionImplementor;
			Session = session;

			FirstLevelCache = new FirstLevelCache();
		}

		private readonly AuditConfiguration verCfg;
		public ISessionImplementor SessionImplementor { get; private set; }
		public ISession Session { get; private set; }
		public FirstLevelCache FirstLevelCache { get; private set; }

		public T Find<T>(object primaryKey, long revision)
		{
			return (T)Find(typeof(T), primaryKey, revision);
		}

		public object Find(System.Type cls, object primaryKey, long revision)
		{
			ArgumentsTools.CheckNotNull(primaryKey, "Primary key");
			ArgumentsTools.CheckNotNull(revision, "Entity revision");
			ArgumentsTools.CheckPositive(revision, "Entity revision");

			var entityName = cls.FullName;

			if (!verCfg.EntCfg.IsVersioned(entityName))
			{
				throw new NotAuditedException(entityName, entityName + " is not versioned!");
			}

			object result;
			if (FirstLevelCache.TryGetValue(entityName, revision, primaryKey, out result))
			{
				return result;
			}

			try
			{
				// The result is put into the cache by the entity instantiator called from the query
				result = CreateQuery().ForEntitiesAtRevision(cls, revision)
					.Add(AuditEntity.Id().Eq(primaryKey)).GetSingleResult();
			}
			catch (NoResultException)
			{
				result = null;
			}

			return result;
		}

		public IEnumerable<long> GetRevisions<TEntity>(object primaryKey) where TEntity : class
		{
			var cls = typeof(TEntity);
			// todo: if a class is not versioned from the beginning, there's a missing ADD rev - what then?
			ArgumentsTools.CheckNotNull(cls, "Entity class");
			ArgumentsTools.CheckNotNull(primaryKey, "Primary key");

			var entityName = cls.FullName;

			if (!verCfg.EntCfg.IsVersioned(entityName))
			{
				throw new NotAuditedException(entityName, entityName + " is not versioned!");
			}

			var resultList = CreateQuery().ForRevisionsOfEntity(cls, false, true)
				.AddProjection(AuditEntity.RevisionNumber())
				.Add(AuditEntity.Id().Eq(primaryKey))
				.GetResultList();
			return from object revision in resultList select Convert.ToInt64(revision);
		}

		public DateTime GetRevisionDate(long revision)
		{
			ArgumentsTools.CheckNotNull(revision, "Entity revision");
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
			ArgumentsTools.CheckNotNull(date, "Date of revision");

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

		public IEnumerable<object> FindEntitiesChangedInRevision(long revision)
		{
			var clazz = FindEntityTypesChangedInRevision(revision);
			var result = new List<object>();
			foreach (var type in clazz)
			{
				result.AddRange(CreateQuery().ForEntitiesModifiedAtRevision(type, revision).GetResultList<object>());
			}
			return result;
		}


		public IEnumerable<object> FindEntitiesChangedInRevision(long revision, RevisionType revisionType)
		{
			var clazz = FindEntityTypesChangedInRevision(revision);
			var result = new List<object>();
			foreach (var type in clazz)
			{
				result.AddRange(CreateQuery().ForEntitiesModifiedAtRevision(type, revision).Add(new RevisionTypeAuditExpression(revisionType, "=")).GetResultList<object>());
			}
			return result;
		}

		public IDictionary<RevisionType, IEnumerable<object>> FindEntitiesChangedInRevisionGroupByRevisionType(long revision)
		{
			var clazz = FindEntityTypesChangedInRevision(revision);
			var result = new Dictionary<RevisionType, IEnumerable<object>>();
			foreach (var revType in Enum.GetValues(typeof(RevisionType)))
			{
				var revisionType = (RevisionType)revType;
				var tempList = new List<object>();
				foreach (var type in clazz)
				{
					var list = CreateQuery().ForEntitiesModifiedAtRevision(type, revision).Add(new RevisionTypeAuditExpression(revisionType, "=")).GetResultList<object>();
					tempList.AddRange(list);
				}
				result[revisionType] = tempList;
			}
			return result;
		}

		public IEnumerable<System.Type> FindEntityTypesChangedInRevision(long revision)
		{
			ArgumentsTools.CheckPositive(revision, "revision");
			if (!verCfg.GlobalCfg.IsTrackEntitiesChangedInRevisionEnabled)
			{
				throw new AuditException(@"This query is designed for Envers default mechanism of tracking entities modified in a given revision." +
											 " Extend DefaultTrackingModifiedTypesRevisionEntity, utilize ModifiedEntityNamesAttribute or set " +
											 "'nhibernate.envers.track_entities_changed_in_revision' parameter to true.");
			}
			var query = verCfg.RevisionInfoQueryCreator.EntitiesChangedInRevisionQuery(Session, revision);
			var modifiedEntityNames = new HashSet<String>(query.List<string>());
			var result = new List<System.Type>(modifiedEntityNames.Count);
			foreach (var modifiedEntityName in modifiedEntityNames)
			{
				result.Add(Toolz.ResolveEntityClass(SessionImplementor, modifiedEntityName));
			}
			return result;
		}

		private void fillRevisionsResult<T>(IDictionary<long, T> result, IEnumerable<long> revisions)
		{
			foreach (var revision in revisions)
			{
				ArgumentsTools.CheckNotNull(revision, "Entity revision");
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
			var sessionAsEventSource = Session as IEventSource;
			if (sessionAsEventSource == null)
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
	}
}
