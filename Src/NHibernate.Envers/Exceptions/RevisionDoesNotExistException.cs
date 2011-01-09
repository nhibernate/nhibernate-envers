using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers.Exceptions
{
    /**
     * @author Adam Warski (adam at warski dot org)
     */
    public class RevisionDoesNotExistException : AuditException {
    	
        public long Revision { get; private set;}
        public DateTime DateTime { get; private set; }

        public RevisionDoesNotExistException(long revision)
            : base("Revision " + revision + " does not exist.")
        {
            this.Revision = revision;
        }

        public RevisionDoesNotExistException(DateTime date)
            : base("There is no revision before or at " + date + ".")
        {
            this.DateTime = date;
        }
    }
}
