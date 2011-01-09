using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Persister.Entity;

namespace NHibernate.Envers.Synchronization.Work
{
    /**
     * @author Adam Warski (adam at warski dot org)
     */
    public class ModWorkUnit: AbstractAuditWorkUnit, IAuditWorkUnit {
        private readonly IDictionary<String, Object> data;
        private readonly bool changes;        

        public ModWorkUnit(ISessionImplementor sessionImplementor, String entityName, AuditConfiguration verCfg, 
					       Object id, IEntityPersister entityPersister, Object[] newState, Object[] oldState)
            : base(sessionImplementor, entityName, verCfg, id)
        {
            data = new Dictionary<String, Object>();
            changes = verCfg.EntCfg[EntityName].PropertyMapper.Map(sessionImplementor, data,
				    entityPersister.PropertyNames, newState, oldState);
        }

        public override bool ContainsWork() {
            return changes;
        }

        public override IDictionary<String, Object> GenerateData(Object revisionData)
        {
            FillDataWithId(data, revisionData, RevisionType.MOD);

            return data;
        }

        public IDictionary<String, Object> getData() {
            return data;
        }

        public override IAuditWorkUnit Merge(AddWorkUnit second)
        {
            return this;
        }

        public override IAuditWorkUnit Merge(ModWorkUnit second)
        {
            return second;
        }

        public override IAuditWorkUnit Merge(DelWorkUnit second)
        {
            return second;
        }

        public override IAuditWorkUnit Merge(CollectionChangeWorkUnit second)
        {
            return this;
        }

        public override IAuditWorkUnit Merge(FakeBidirectionalRelationWorkUnit second)
        {
            return FakeBidirectionalRelationWorkUnit.merge(second, this, second.getNestedWorkUnit());
        }

        public override IAuditWorkUnit Dispatch(IWorkUnitMergeVisitor first)
        {
            return first.Merge(this);
        }
    }
}
