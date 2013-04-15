using System.Collections;
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
		/// Creates a new <see cref="IPropertyMapper"/> for an 
		/// <see cref="System.Collections.IList"/> with identifier bag semantics.
		/// </summary>
		IPropertyMapper IdBag(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, bool embeddableElementType);


		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="Iesi.Collections.Generic.ISet{T}" />.
		/// </summary>
		IPropertyMapper Set<T>(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, bool ordinalInId, bool embeddableElementType);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="Iesi.Collections.ISet" />.
		/// </summary>
		IPropertyMapper Set(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, bool ordinalInId, bool embeddableElementType);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="Iesi.Collections.ISet" />
		/// that is sorted by an <see cref="IComparer"/>.
		/// </summary>
		IPropertyMapper SortedSet(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, IComparer comparer, bool ordinalInId, bool embeddableElementType);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="Iesi.Collections.ISet" />
		/// that is sorted by an <see cref="IComparer{T}"/>.
		/// </summary>
		IPropertyMapper SortedSet<T>(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, IComparer<T> comparer, bool ordinalInId, bool embeddableElementType);


		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="System.Collections.Generic.IList{T}" />.
		/// </summary>
		IPropertyMapper List<T>(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, bool embeddableElementType);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="System.Collections.IList" />.
		/// </summary>
		IPropertyMapper List(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, bool embeddableElementType);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="System.Collections.Generic.IDictionary{K, T}" />.
		/// </summary>
		IPropertyMapper Map<TKey, TValue>(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, bool embeddableElementType);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="IDictionary" />.
		/// </summary>
		IPropertyMapper Map(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, bool embeddableElementType);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="IDictionary"/>
		/// that is sorted by an <see cref="IComparer"/>.
		/// </summary>
		IPropertyMapper SortedMap(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, IComparer comparer, bool embeddableElementType);

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

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an 
		/// <see cref="System.Collections.IList"/> with bag semantics.
		/// </summary>
		IPropertyMapper Bag(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, bool embeddableElementType);
	}
}