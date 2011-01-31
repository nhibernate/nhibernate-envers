using System;
using System.Collections.Generic;
using System.Reflection;

namespace NHibernate.Envers.Configuration.Store
{
	/// <summary>
	/// Holds configuration information for an entity
	/// </summary>
	public interface IEntityMeta
	{
		IEnumerable<Attribute> ClassMetas { get; }
		IDictionary<MemberInfo, IEnumerable<Attribute>> MemberMetas { get; }
	}
}