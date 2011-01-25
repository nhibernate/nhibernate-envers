using System;

namespace NHibernate.Envers
{
	/// <summary>
	///  Marks a property which will hold the number of the revision in a revision entity, see
	///  {@link RevisionListener}. Values of this property should form a strictly-increasing sequence
	///  of numbers. The value of this property won't be set by Envers. In most cases, this should be
	///  an auto-generated database-assigned primary id.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class RevisionNumberAttribute : Attribute
	{
	}
}
