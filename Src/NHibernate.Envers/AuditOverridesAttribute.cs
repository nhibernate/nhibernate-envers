using System;

namespace NHibernate.Envers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Method)]
    public class AuditOverridesAttribute:Attribute
    {
        /**
         * @return An array of {@link AuditOverride} values, to define the new auditing
         * behavior.
         */
        public AuditOverrideAttribute[] value;
    }
}
