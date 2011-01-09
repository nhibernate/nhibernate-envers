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
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public class AddWorkUnit : AbstractAuditWorkUnit, IAuditWorkUnit {
    private readonly IDictionary<String, Object> data;

    public AddWorkUnit(ISessionImplementor sessionImplementor, String entityName, AuditConfiguration verCfg,
					   Object id, IEntityPersister entityPersister, Object[] state) 
        : base(sessionImplementor, entityName, verCfg, id)
    {
        data = new Dictionary<String, Object>();
        verCfg.EntCfg[EntityName].PropertyMapper.Map(sessionImplementor, data,
				entityPersister.PropertyNames, state, null);
    }

    public AddWorkUnit(ISessionImplementor sessionImplementor, String entityName, AuditConfiguration verCfg,
                       Object id, IDictionary<String, Object> data)
        : base(sessionImplementor, entityName, verCfg, id)
    {
        this.data = data;
    }

    public override bool ContainsWork()
    {
        return true;
    }

    public override IDictionary<String, Object> GenerateData(Object revisionData)
    {
        FillDataWithId(data, revisionData, RevisionType.ADD);
        return data;
    }

    public override IAuditWorkUnit Merge(AddWorkUnit second)
    {
        return second;
    }

    public override IAuditWorkUnit Merge(ModWorkUnit second)
    {
        return new AddWorkUnit(sessionImplementor, EntityName, verCfg, EntityId, second.getData());
    }

    public override IAuditWorkUnit Merge(DelWorkUnit second)
    {
        return null;
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
