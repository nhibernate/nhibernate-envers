using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers.Synchronization.Work
{
    /**
     * Visitor patter dispatcher.
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public interface IWorkUnitMergeDispatcher
    {
        /**
         * Shuold be invoked on the second work unit.
         * @param first First work unit (that is, the one added earlier).
         * @return The work unit that is the result of the merge.
         */
        IAuditWorkUnit Dispatch(IWorkUnitMergeVisitor first);
    }
}
