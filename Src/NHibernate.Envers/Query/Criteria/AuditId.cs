using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Query.Projection;
using NHibernate.Envers.Query.Property;

namespace NHibernate.Envers.Query.Criteria
{
    /**
     * Create restrictions and projections for the id of an audited entity.
     * @author Adam Warski (adam at warski dot org)
     */
    public class AuditId {
        /**
	     * Apply an "equal" constraint
	     */
	    public IAuditCriterion Eq(Object id) {
		    return new IdentifierEqAuditExpression(id, true);
	    }

        /**
	     * Apply a "not equal" constraint
	     */
	    public IAuditCriterion ne(Object id) {
		    return new IdentifierEqAuditExpression(id, false);
	    }

        // Projections

        /**
         * Projection counting the values
         * TODO: idPropertyName isn't needed, should be read from the configuration
         * @param idPropertyName Name of the identifier property
         */
        public IAuditProjection count(String idPropertyName) {
            return new PropertyAuditProjection(new OriginalIdPropertyName(idPropertyName), "count", false);
        }
    }
}
