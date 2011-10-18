using System;

namespace NHibernate.Envers.Configuration.Attributes
{
	/// <summary>
	/// This annotation expects property of <code><![CDATA[ISet<string>]]></code> type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public sealed class ModifiedEntityTypesAttribute : Attribute
	{
	}
}