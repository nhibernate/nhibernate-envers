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
    public class AuditConjunction : IAuditCriterion, IExtendableCriterion {
        private IList<IAuditCriterion> criterions;

        public AuditConjunction() {
            criterions = new List<IAuditCriterion>();
        }

        public IExtendableCriterion Add(IAuditCriterion criterion) {
            criterions.Add(criterion);
            return this;
        }

        public void AddToQuery(AuditConfiguration verCfg, String entityName, QueryBuilder qb, Parameters parameters) {
            Parameters andParameters = parameters.AddSubParameters(Parameters.AND);

            if (criterions.Count == 0) {
                andParameters.AddWhere("1", false, "=", "1", false);
            } else {
                foreach (IAuditCriterion criterion in criterions) {
                    criterion.AddToQuery(verCfg, entityName, qb, andParameters);
                }
            }
        }
    }
}
