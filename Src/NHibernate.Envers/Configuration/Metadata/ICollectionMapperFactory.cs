using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Relation;
using System.Collections.Generic;

namespace NHibernate.Envers.Configuration.Metadata
{
	/// <summary>
	/// Mapper factory for collection.
	/// Users can implement their own by setting <see cref="ConfigurationKey.CollectionMapperFactory"/>.
	/// </summary>
	/// <remarks>
	/// Implementations of this interface must have a public, default ctor
	/// </remarks>
	public interface ICollectionMapperFactory
	{
		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="System.Array" />.
		/// </summary>
		IPropertyMapper Array(IEnversProxyFactory enversProxyFactory, System.Type elementType, CommonCollectionMapperData commonCollectionMapperData,
		                      MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, bool embeddableElementType);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an 
		/// <see cref="System.Collections.Generic.IList{T}"/> with identifier bag semantics.
		/// </summary>
		IPropertyMapper IdBag<T>(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, bool embeddableElementType);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="ISet{T}" />.
		/// </summary>
		IPropertyMapper Set<T>(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, bool embeddableElementType);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="ISet{T}" />
		/// that is sorted by an <see cref="IComparer{T}"/>.
		/// </summary>
		IPropertyMapper SortedSet<T>(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, IComparer<T> comparer, bool embeddableElementType);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="System.Collections.Generic.IList{T}" />.
		/// </summary>
		IPropertyMapper List<T>(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, bool embeddableElementType);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="System.Collections.Generic.IDictionary{K, T}" />.
		/// </summary>
		IPropertyMapper Map<TKey, TValue>(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, bool embeddableElementType);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="IDictionary{K, V}"/>
		/// that is sorted by an <see cref="IComparer{K}"/>.
		/// </summary>
		IPropertyMapper SortedMap<TKey, TValue>(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, IComparer<TKey> comparer, bool embeddableElementType);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an 
		/// <see cref="System.Collections.Generic.IList{T}"/> with bag semantics.
		/// </summary>
		IPropertyMapper Bag<T>(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, bool embeddableElementType);
	}
}