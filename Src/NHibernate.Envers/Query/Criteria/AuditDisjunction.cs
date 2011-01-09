using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query.Criteria
{
    /**
     * @author Adam Warski (adam at warski dot org)
     */
    public class AuditDisjunction : IAuditCriterion, IExtendableCriterion {
        private IList<IAuditCriterion> criterions;

        public AuditDisjunction() {
            criterions = new List<IAuditCriterion>();
        }

        public IExtendableCriterion Add(IAuditCriterion criterion) {
            criterions.Add(criterion);
            return this;
        }

        public void AddToQuery(AuditConfiguration verCfg, String entityName, QueryBuilder qb, Parameters parameters) {
            Parameters orParameters = parameters.AddSubParameters(Parameters.OR);

            if (criterions.Count == 0) {
                orParameters.AddWhere("0", false, "=", "1", false);
            } else {
                foreach (IAuditCriterion criterion in criterions) {
                    criterion.AddToQuery(verCfg, entityName, qb, orParameters);
                }
            }
        }
    }
}
