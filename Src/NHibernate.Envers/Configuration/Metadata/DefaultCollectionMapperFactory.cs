using System;
using System.Collections.Generic;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Relation;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy;

namespace NHibernate.Envers.Configuration.Metadata
{
	[Serializable]
	public class DefaultCollectionMapperFactory : ICollectionMapperFactory
	{
		public virtual IPropertyMapper Array(System.Type elementType, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, bool embeddableElementType)
		{
			throw new NotImplementedException("Array is not supported by DefaultCollectionMapperFactory");
		}

		public virtual IPropertyMapper IdBag<T>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, bool embeddableElementType)
		{
			return new IdBagCollectionMapper<T>(commonCollectionMapperData, typeof(ListProxy<T>), elementComponentData, embeddableElementType);
		}

		public virtual IPropertyMapper Set<T>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, bool embeddableElementType)
		{
			return new SetCollectionMapper<T>(commonCollectionMapperData, typeof (SetProxy<T>), elementComponentData, embeddableElementType, embeddableElementType);
		}

		public virtual IPropertyMapper SortedSet<T>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, IComparer<T> comparer, bool embeddableElementType)
		{
			return new SortedSetCollectionMapper<T>(commonCollectionMapperData, typeof(SetProxy<T>), elementComponentData, comparer, embeddableElementType, embeddableElementType);
		}

		public virtual IPropertyMapper List<T>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, bool embeddableElementType)
		{
			return new ListCollectionMapper<T>(commonCollectionMapperData, typeof(ListProxy<T>), elementComponentData, indexComponentData, embeddableElementType);
		}

		public virtual IPropertyMapper Map<TKey, TValue>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, bool embeddableElementType)
		{
			return new MapCollectionMapper<TKey, TValue>(commonCollectionMapperData, typeof(DictionaryProxy<TKey, TValue>), elementComponentData, indexComponentData, embeddableElementType);
		}

		public virtual IPropertyMapper SortedMap<TKey, TValue>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, IComparer<TKey> comparer, bool embeddableElementType)
		{
			return new SortedMapCollectionMapper<TKey, TValue>(commonCollectionMapperData, typeof(DictionaryProxy<TKey, TValue>), elementComponentData, indexComponentData, comparer, embeddableElementType);
		}

		public virtual IPropertyMapper Bag<T>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, bool embeddableElementType)
		{
			return new BagCollectionMapper<T>(commonCollectionMapperData, typeof(ListProxy<T>), elementComponentData, embeddableElementType);
		}
	}
}