using System;

namespace NHibernate.Envers.Configuration.Attributes
{
	/// <summary>
	/// Marks a property which holds entity class names that have been modified during each revision.
	/// This annotation expects property of <code><![CDATA[ISet<string>]]></code> type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class ModifiedEntityNamesAttribute : Attribute
	{
	}
}