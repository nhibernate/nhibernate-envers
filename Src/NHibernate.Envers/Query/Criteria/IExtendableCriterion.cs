using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers.Query.Criteria
{
    /**
     * @author Adam Warski (adam at warski dot org)
     */
    public interface IExtendableCriterion
    {
        IExtendableCriterion Add(IAuditCriterion criterion);
    }
}
