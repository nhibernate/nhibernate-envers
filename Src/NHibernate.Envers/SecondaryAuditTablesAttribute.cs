using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers
{
    class SecondaryAuditTablesAttribute:Attribute
    {
        public JoinAuditTableAttribute[] Value { get; set; }
    }
}
