using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Relation;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy;

namespace NHibernate.Envers.Configuration.Metadata
{
	public class DefaultCollectionProxyMapperFactory : ICollectionProxyMapperFactory
	{
		public virtual IPropertyMapper Array(System.Type elementType, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData)
		{
			throw new NotImplementedException("Array is not supported by DefaultCollectionProxyMapperFactory");
		}

		public virtual IPropertyMapper IdBag<T>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData)
		{
			throw new NotImplementedException("Generic idbag is not supported by DefaultCollectionProxyMapperFactory");
		}

		public virtual IPropertyMapper IdBag(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData)
		{
			throw new NotImplementedException("Non generic idbag is not supported by DefaultCollectionProxyMapperFactory");
		}

		public virtual IPropertyMapper Set<T>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData)
		{
			return new SetCollectionMapper<T>(commonCollectionMapperData, 
			                                  typeof (SetProxy<T>),
			                                  elementComponentData);
		}

		public virtual IPropertyMapper Set(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData)
		{
			throw new NotImplementedException("Non generic set is not supported by DefaultCollectionProxyMapperFactory");
		}

		public virtual IPropertyMapper SortedSet(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, IComparer comparer)
		{
			throw new NotImplementedException("Non generic sorted set is not supported by DefaultCollectionProxyMapperFactory");
		}

		public virtual IPropertyMapper SortedSet<T>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, IComparer<T> comparer)
		{
			return new SortedSetCollectionMapper<T>(commonCollectionMapperData,
											 typeof(SetProxy<T>),
											 elementComponentData,
											 comparer);
		}

		public virtual IPropertyMapper List<T>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData)
		{
			return new ListCollectionMapper<T>(commonCollectionMapperData, elementComponentData, indexComponentData);
		}

		public virtual IPropertyMapper List(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData)
		{
			throw new NotImplementedException("Non generic list is not supported by DefaultCollectionProxyMapperFactory");
		}

		public virtual IPropertyMapper Map<TKey, TValue>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData)
		{
			return new MapCollectionMapper<TKey, TValue>(commonCollectionMapperData, typeof(MapProxy<TKey, TValue>), elementComponentData, indexComponentData);
		}

		public virtual IPropertyMapper Map(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData)
		{
			throw new NotImplementedException("Non generic map is not supported by DefaultCollectionProxyMapperFactory");
		}

		public virtual IPropertyMapper SortedMap(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, IComparer comparer)
		{
			throw new NotImplementedException("Non generic sorted map is not supported by DefaultCollectionProxyMapperFactory");
		}

		public virtual IPropertyMapper SortedMap<TKey, TValue>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, IComparer<TKey> comparer)
		{
			return new SortedMapCollectionMapper<TKey, TValue>(commonCollectionMapperData, typeof(MapProxy<TKey, TValue>), elementComponentData, indexComponentData, comparer);
		}

		public virtual IPropertyMapper Bag<T>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData)
		{
			return new BagCollectionMapper<T>(commonCollectionMapperData, elementComponentData);
		}

		public virtual IPropertyMapper Bag(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData)
		{
			throw new NotImplementedException("Non generic bag is not supported by DefaultCollectionProxyMapperFactory");
		}
	}
}