using System;

namespace NHibernate.Envers.Configuration.Attributes
{
	/// <summary>
	/// Marks a property which will hold the timestamp of the revision in a revision entity, see
	/// <see cref="IRevisionListener"/>. The value of this property will be automatically set by Envers.
	/// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public sealed class RevisionTimestampAttribute : Attribute
    {
    }
}
