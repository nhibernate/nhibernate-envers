using System;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;

namespace NHibernate.Envers.Synchronization.Work
{
    public abstract class AbstractAuditWorkUnit : IAuditWorkUnit 
	{
	    protected readonly ISessionImplementor sessionImplementor;
        protected readonly AuditConfiguration verCfg;
        public object EntityId { get; private set; }
        public string EntityName { get; private set; }

        public object performedData;

        protected AbstractAuditWorkUnit(ISessionImplementor sessionImplementor,			
										string entityName, 
										AuditConfiguration verCfg,
									    object id) 
		{
		    this.sessionImplementor = sessionImplementor;
            this.verCfg = verCfg;
            EntityId = id;
            EntityName = entityName;
        }

        protected void FillDataWithId(IDictionary<string, object> data, object revision, RevisionType revisionType) 
		{
            var entitiesCfg = verCfg.AuditEntCfg;

            var originalId = new Dictionary<String, Object> {{entitiesCfg.RevisionFieldName, revision}};

        	verCfg.EntCfg[EntityName].GetIdMapper().MapToMapFromId(originalId, EntityId);
            data.Add(entitiesCfg.RevisionTypePropName, revisionType);
            data.Add(entitiesCfg.OriginalIdPropName, originalId);
        }

        public virtual void Perform(ISession session, object revisionData) 
		{
            var data = GenerateData(revisionData);
            session.Save(verCfg.AuditEntCfg.GetAuditEntityName(EntityName), data);
            SetPerformed(data);
        }

        public bool IsPerformed() 
		{
            return performedData != null;
        }

        protected void SetPerformed(object performedData) 
		{
            this.performedData = performedData;
        }

        public void Undo(ISession session) 
		{
            if (IsPerformed()) 
			{
                session.Delete(verCfg.AuditEntCfg.GetAuditEntityName(EntityName), performedData);
                session.Flush();
            }
        }

        public abstract IAuditWorkUnit Dispatch(IWorkUnitMergeVisitor first);
		public abstract IAuditWorkUnit Merge(AddWorkUnit second);
        public abstract IAuditWorkUnit Merge(ModWorkUnit second);
        public abstract IAuditWorkUnit Merge(DelWorkUnit second);
        public abstract IAuditWorkUnit Merge(CollectionChangeWorkUnit second);
        public abstract IAuditWorkUnit Merge(FakeBidirectionalRelationWorkUnit second);
		public abstract IDictionary<string, object> GenerateData(object revisionData);
        public abstract bool ContainsWork();
    }
}
