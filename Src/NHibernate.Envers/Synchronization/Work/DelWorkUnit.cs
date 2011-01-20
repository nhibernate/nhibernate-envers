using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Entities;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Persister.Entity;

namespace NHibernate.Envers.Synchronization.Work
{
    /**
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public class DelWorkUnit : AbstractAuditWorkUnit, IAuditWorkUnit {
        private readonly Object[] state;
        private readonly String[] propertyNames;

        public DelWorkUnit(ISessionImplementor sessionImplementor, String entityName, AuditConfiguration verCfg,
					       Object id, IEntityPersister entityPersister, Object[] state)
            : base(sessionImplementor, entityName, verCfg, id)
        {
            this.state = state;
            this.propertyNames = entityPersister.PropertyNames;
        }

        public override bool ContainsWork()
        {
            return true;
        }

        public override IDictionary<String, Object> GenerateData(Object revisionData)
        {
            IDictionary<String, Object> data = new Dictionary<String, Object>();
            FillDataWithId(data, revisionData, RevisionType.DEL);

		    if (verCfg.GlobalCfg.StoreDataAtDelete) {
			    verCfg.EntCfg[EntityName].PropertyMapper.Map(sessionImplementor, data,
					    propertyNames, state, state);
		    }

            return data;
        }

        public override IAuditWorkUnit Merge(AddWorkUnit second)
        {
            return null;
        }

        public override IAuditWorkUnit Merge(ModWorkUnit second)
        {
            return null;
        }

        public override IAuditWorkUnit Merge(DelWorkUnit second)
        {
            return this;
        }

        public override IAuditWorkUnit Merge(CollectionChangeWorkUnit second)
        {
            return this;
        }

        public override IAuditWorkUnit Merge(FakeBidirectionalRelationWorkUnit second)
        {
            return this;
        }

        public override IAuditWorkUnit Dispatch(IWorkUnitMergeVisitor first)
        {
            return first.Merge(this);
        }
    }
}

