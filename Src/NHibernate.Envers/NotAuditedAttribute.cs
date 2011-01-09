using System;

namespace NHibernate.Envers
{
    /**
     * When applied to a field, indicates that this field should not be audited.
     * @author Simon Duduica, port of Envers omonyme class by Sebastian Komander
     */
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method)]
    public class NotAuditedAttribute : Attribute
    {
    }
}
