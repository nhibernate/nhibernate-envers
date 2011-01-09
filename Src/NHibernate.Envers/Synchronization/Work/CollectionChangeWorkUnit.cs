using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;

namespace NHibernate.Envers.Synchronization.Work
{
    /**
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public class CollectionChangeWorkUnit : AbstractAuditWorkUnit, IAuditWorkUnit {
        private readonly Object entity;

        public CollectionChangeWorkUnit(ISessionImplementor session, String entityName, AuditConfiguration verCfg,
									    Object id, Object entity)
            : base(session, entityName, verCfg, id)
        {
            this.entity = entity;
        }

        public override bool ContainsWork()
        {
            return true;
        }

        public override IDictionary<String, Object> GenerateData(Object revisionData)
        {
            IDictionary<String, Object> data = new Dictionary<String, Object>();
            FillDataWithId(data, revisionData, RevisionType.MOD);

            verCfg.EntCfg[EntityName].PropertyMapper.MapToMapFromEntity(sessionImplementor,
				    data, entity, null);

            return data;
        }

        public override IAuditWorkUnit Merge(AddWorkUnit second) {
            return second;
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
            return second;
        }

        public override IAuditWorkUnit Dispatch(IWorkUnitMergeVisitor first)
        {
            return first.Merge(this);
        }
    }
}
