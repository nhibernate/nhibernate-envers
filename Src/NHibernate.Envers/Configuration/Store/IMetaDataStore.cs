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
		/// <summary>
		/// All entity metas keyed by type.
		/// </summary>
		IDictionary<System.Type, IEntityMeta> EntityMetas { get; }

		/// <summary>
		/// Gets attribute for certain type.
		/// </summary>
		/// <typeparam name="T">Attribute type</typeparam>
		/// <param name="entityType">Type to search</param>
		/// <returns>
		/// Only one attribute will be returned, 
		/// if multiple exists you'll need to use <see cref="EntityMetas"/>
		/// If attribute does not exists, null is returned.
		/// </returns>
		T ClassMeta<T>(System.Type entityType) where T : Attribute;

		/// <summary>
		/// Gets attribute for certain member.
		/// <typeparam name="T">Attribute type</typeparam>
		/// </summary>
		/// <param name="member">The member to search</param>
		/// <returns>
		/// Only one attribute will be returned, 
		/// if multiple exists you'll need to use <see cref="EntityMetas"/>
		/// If attribute does not exists, null is returned.
		/// </returns>
		T MemberMeta<T>(MemberInfo member) where T : Attribute;

		/// <summary>
		/// Gets all types decorated with attribute T
		/// </summary>
		/// <typeparam name="T">Attribute type</typeparam>
		/// <returns>The types</returns>
		IEnumerable<System.Type> EntitiesDeclaredWith<T>() where T : Attribute;
	}
}