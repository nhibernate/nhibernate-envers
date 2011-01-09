using System;

namespace NHibernate.Envers
{
    /**
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    [AttributeUsage(AttributeTargets.Class)]
    class SecondaryAuditTableAttribute : Attribute
    {
        public String secondaryTableName;
        public String secondaryAuditTableName;
    }
}
