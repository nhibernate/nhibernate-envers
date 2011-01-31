using System;
using System.Collections.Generic;
using System.Reflection;

namespace NHibernate.Envers.Configuration.Store
{
	/// <summary>
	/// Holds configuration data for all entities
	/// </summary>
	public interface IMetaDataStore
	{
		T ClassMeta<T>(System.Type entityType) where T : Attribute;
		T MemberMeta<T>(MemberInfo member) where T : Attribute;
		IDictionary<System.Type, IEntityMeta> EntityMetas { get; }
	}
}