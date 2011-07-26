using System.Collections;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Relation;
using System.Collections.Generic;

namespace NHibernate.Envers.Configuration.Metadata
{
	/// <summary>
	/// Mapper factory for collection proxies
	/// </summary>
	/// <remarks>
	/// Implementations of this interface must have a public, default ctor
	/// </remarks>
	public interface ICollectionProxyMapperFactory
	{
		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="System.Array" />.
		/// </summary>
		IPropertyMapper Array(System.Type elementType, CommonCollectionMapperData commonCollectionMapperData,
		                      MiddleComponentData elementComponentData, MiddleComponentData indexComponentData);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an 
		/// <see cref="System.Collections.Generic.IList{T}"/> with identifier bag semantics.
		/// </summary>
		IPropertyMapper IdBag<T>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an 
		/// <see cref="System.Collections.IList"/> with identifier bag semantics.
		/// </summary>
		IPropertyMapper IdBag(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData);


		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="Iesi.Collections.Generic.ISet{T}" />.
		/// </summary>
		IPropertyMapper Set<T>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="Iesi.Collections.ISet" />.
		/// </summary>
		IPropertyMapper Set(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="Iesi.Collections.ISet" />
		/// that is sorted by an <see cref="IComparer"/>.
		/// </summary>
		IPropertyMapper SortedSet(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, IComparer comparer);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="Iesi.Collections.ISet" />
		/// that is sorted by an <see cref="IComparer{T}"/>.
		/// </summary>
		IPropertyMapper SortedSet<T>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, IComparer<T> comparer);


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
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="IDictionary" />.
		/// </summary>
		IPropertyMapper Map(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="IDictionary"/>
		/// that is sorted by an <see cref="IComparer"/>.
		/// </summary>
		IPropertyMapper SortedMap(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, IComparer comparer);

		/// <summary>
		/// Creates a new <see cref="IPropertyMapper"/> for an <see cref="IDictionary{K, V}"/>
		/// that is sorted by an <see cref="IComparer{K}"/>.
		/// </summary>
		IPropertyMapper SortedMap<TKey, TValue>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, IComparer<TKey> comparer);


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