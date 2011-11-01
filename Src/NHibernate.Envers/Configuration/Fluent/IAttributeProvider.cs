using System.Collections.Generic;

namespace NHibernate.Envers.Configuration.Fluent
{
	/// <summary>
	/// Creates attributes based on fluent configuration.
	/// </summary>
	public interface IAttributeProvider
	{
		/// <summary>
		/// The entity type this configuration is for
		/// </summary>
		System.Type Type { get; }

		/// <summary>
		/// Creates corresponding member attributes
		/// </summary>
		/// <returns>The attributes</returns>
		IEnumerable<MemberInfoAndAttribute> Attributes();
	}
}