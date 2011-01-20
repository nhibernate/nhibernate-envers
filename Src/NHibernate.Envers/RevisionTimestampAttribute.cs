using System;

namespace NHibernate.Envers
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class RevisionTimestampAttribute : Attribute
    {
    }
}
