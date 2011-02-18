using System;
using System.Collections.Generic;
using System.Reflection;

namespace NHibernate.Envers.Configuration.Store
{
	/// <summary>
	/// Holds configuration information for an entity type
	/// </summary>
	public interface IEntityMeta
	{
		/// <summary>
		/// Metas for current class
		/// </summary>
		IEnumerable<Attribute> ClassMetas { get; }

		/// <summary>
		/// Member metas for current class
		/// </summary>
		IDictionary<MemberInfo, IEnumerable<Attribute>> MemberMetas { get; }
	}
}