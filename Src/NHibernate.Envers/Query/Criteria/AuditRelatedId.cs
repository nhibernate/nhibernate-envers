using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Query.Property;

namespace NHibernate.Envers.Query.Criteria
{
    /**
     * Create restrictions on an id of an entity related to an audited entity.
     * @author Adam Warski (adam at warski dot org)
     */
    public class AuditRelatedId {
        private readonly IPropertyNameGetter propertyNameGetter;

        public AuditRelatedId(IPropertyNameGetter propertyNameGetter) {
            this.propertyNameGetter = propertyNameGetter;
        }

        /**
	     * Apply an "equal" constraint
	     */
	    public IAuditCriterion eq(Object id) {
		    return new RelatedAuditExpression(propertyNameGetter, id, true);
	    }

        /**
	     * Apply a "not equal" constraint
	     */
	    public IAuditCriterion ne(Object id) {
		    return new RelatedAuditExpression(propertyNameGetter, id, false);
	    }
    }
}
