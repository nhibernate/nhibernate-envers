using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers.Synchronization.Work
{
    /**
     * Visitor pattern visitor. All methods should be invoked on the first work unit.
     * @author Adam Warski (adam at warski dot org)
     */
    public interface IWorkUnitMergeVisitor
    {
        IAuditWorkUnit Merge(AddWorkUnit second);
        IAuditWorkUnit Merge(ModWorkUnit second);
        IAuditWorkUnit Merge(DelWorkUnit second);
        IAuditWorkUnit Merge(CollectionChangeWorkUnit second);
        IAuditWorkUnit Merge(FakeBidirectionalRelationWorkUnit second);
    }
}
