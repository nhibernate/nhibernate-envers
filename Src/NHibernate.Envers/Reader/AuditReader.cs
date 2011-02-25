﻿using System;
using System.Collections;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tools;
using NHibernate.Event;

namespace NHibernate.Envers.Reader
{
	public class AuditReader : IAuditReaderImplementor 
	{
		private readonly AuditConfiguration verCfg;
		public ISessionImplementor SessionImplementor{ get; private set;}
		public ISession Session{ get; private set;}
		public IFirstLevelCache FirstLevelCache{ get; private set;}

		public AuditReader(AuditConfiguration verCfg, ISession session,
								  ISessionImplementor sessionImplementor) 
		{
			this.verCfg = verCfg;
			SessionImplementor = sessionImplementor;
			Session = session;

			FirstLevelCache = new FirstLevelCache();
		}

		private void CheckSession() 
		{
			if (!Session.IsOpen) 
			{
				throw new Exception("The associated session is closed!");
			}
		}

		public T Find<T> (object primaryKey, long revision)
		{
			return (T) Find(typeof (T), primaryKey, revision);
		}

		public object Find(System.Type cls, object primaryKey, long revision)
		{
			ArgumentsTools.CheckNotNull(primaryKey, "Primary key");
			ArgumentsTools.CheckNotNull(revision, "Entity revision");
			ArgumentsTools.CheckPositive(revision, "Entity revision");
			CheckSession();

			var entityName = cls.FullName;

			if (!verCfg.EntCfg.IsVersioned(entityName))
			{
				throw new NotAuditedException(entityName, entityName + " is not versioned!");
			}

			if (FirstLevelCache.Contains(entityName, revision, primaryKey))
			{
				return FirstLevelCache[entityName, revision, primaryKey];
			}

			object result;
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

		public IList GetRevisions(System.Type cls, object primaryKey)
		{
			// todo: if a class is not versioned from the beginning, there's a missing ADD rev - what then?
			ArgumentsTools.CheckNotNull(cls, "Entity class");
			ArgumentsTools.CheckNotNull(primaryKey, "Primary key");
			CheckSession();

			var entityName = cls.FullName;

			if (!verCfg.EntCfg.IsVersioned(entityName)) 
			{
				throw new NotAuditedException(entityName, entityName + " is not versioned!");
			}

			return CreateQuery().ForRevisionsOfEntity(cls, false, true)
					.AddProjection(AuditEntity.RevisionNumber())
					.Add(AuditEntity.Id().Eq(primaryKey))
					.GetResultList();
		}

		public DateTime GetRevisionDate(long revision)
		{
			ArgumentsTools.CheckNotNull(revision, "Entity revision");
			ArgumentsTools.CheckPositive(revision, "Entity revision");
			CheckSession();

			var query = verCfg.RevisionInfoQueryCreator.RevisionDateQuery(Session, revision);

			var timestampObject = query.UniqueResult();
			if (timestampObject == null) 
			{
				throw new RevisionDoesNotExistException(revision);
			}

			// The timestamp object is either a date or a long
			return timestampObject is DateTime ? (DateTime) timestampObject : new DateTime((long) timestampObject);
		}

		public long GetRevisionNumberForDate(DateTime date) 
		{
			ArgumentsTools.CheckNotNull(date, "Date of revision");
			CheckSession();

			var query = verCfg.RevisionInfoQueryCreator.RevisionNumberForDateQuery(Session, date);

			var res = query.UniqueResult();
			if (res == null) 
			{
				throw new RevisionDoesNotExistException(date);
			}

			return Convert.ToInt64(res);
		}


		public object FindRevision(System.Type type, long revision)
		{
			ArgumentsTools.CheckNotNull(revision, "Entity revision");
			ArgumentsTools.CheckPositive(revision, "Entity revision");
			CheckSession();

			var query = verCfg.RevisionInfoQueryCreator.RevisionQuery(Session, revision);

			var revisionData = query.UniqueResult();

			if (revisionData == null)
			{
				throw new RevisionDoesNotExistException(revision);
			}

			return revisionData;
		}

		public T FindRevision<T>(long revision)
		{
			return (T) FindRevision(typeof (T), revision);
		}

		public object GetCurrentRevision(System.Type type, bool persist)
		{
			if (!(Session is IEventSource))
			{
				throw new NotSupportedException("The provided session is not an EventSource!");// ORIG IllegalArgumentException
			}

			// Obtaining the current audit sync
			var auditSync = verCfg.AuditSyncManager.get((IEventSource)Session);

			// And getting the current revision data
			return auditSync.GetCurrentRevisionData(Session, persist);
		}

		public T GetCurrentRevision<T>(bool persist)
		{
			return (T) GetCurrentRevision(typeof(T), persist);
		}

		public AuditQueryCreator CreateQuery() 
		{
			return new AuditQueryCreator(verCfg, this);
		}
	}
}
