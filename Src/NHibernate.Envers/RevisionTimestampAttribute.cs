using System;

namespace NHibernate.Envers
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class RevisionTimestampAttribute : Attribute
    {
    }
}
