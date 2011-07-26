using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Relation;

namespace NHibernate.Envers.Configuration.Metadata
{
	/// <summary>
	/// Mapper factory for collection proxies
	/// </summary>
	public interface ICollectionProxyMapperFactory
	{
		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="Iesi.Collections.Generic.ISet{T}" />.
		/// </summary>
		IPropertyMapper Set<T>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="Iesi.Collections.ISet" />.
		/// </summary>
		IPropertyMapper Set(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="System.Collections.Generic.IList{T}" />.
		/// </summary>
		IPropertyMapper List<T>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="System.Collections.IList" />.
		/// </summary>
		IPropertyMapper List(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="System.Collections.Generic.IDictionary{K, T}" />.
		/// </summary>
		IPropertyMapper Map<TKey, TValue>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="System.Collections.IDictionary" />.
		/// </summary>
		IPropertyMapper Map(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an 
		/// <see cref="System.Collections.Generic.IList{T}"/> with bag semantics.
		/// </summary>
		IPropertyMapper Bag<T>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an 
		/// <see cref="System.Collections.IList"/> with bag semantics.
		/// </summary>
		IPropertyMapper Bag(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData);
	}
}