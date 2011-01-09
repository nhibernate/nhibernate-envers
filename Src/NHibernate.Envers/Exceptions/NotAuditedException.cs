using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers.Exceptions
{
    /**
     * @author Adam Warski (adam at warski dot org)
     */
    public class NotAuditedException : AuditException {
    	
        public String EntityName{ get; private set;}

	    public NotAuditedException(String entityName, String message):base(message) {
            this.EntityName = entityName;
        }
    }
}
