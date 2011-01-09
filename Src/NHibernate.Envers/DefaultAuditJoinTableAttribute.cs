using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers
{
    class DefaultAuditJoinTableAttribute : AuditJoinTableAttribute
    {
        public System.Type AttributeType()
        {
            return typeof(DefaultAuditJoinTableAttribute);
        }

    }
}
