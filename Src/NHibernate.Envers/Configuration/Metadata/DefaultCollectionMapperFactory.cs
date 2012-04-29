using System;
using System.Collections;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Relation;

namespace NHibernate.Envers.Configuration.Metadata
{
	public class DefaultCollectionMapperFactory : ICollectionMapperFactory
	{
		private IEnversProxyFactory _proxyEnversFactory;

		public void Initialize(IEnversProxyFactory enversProxyFactory)
		{
			_proxyEnversFactory = enversProxyFactory;
		}

		public virtual IPropertyMapper Array(System.Type elementType, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData)
		{
			throw new NotImplementedException("Array is not supported by DefaultCollectionMapperFactory");
		}

		public virtual IPropertyMapper IdBag<T>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData)
		{
			throw new NotImplementedException("Generic idbag is not supported by DefaultCollectionMapperFactory");
		}

		public virtual IPropertyMapper IdBag(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData)
		{
			throw new NotImplementedException("Non generic idbag is not supported by DefaultCollectionMapperFactory");
		}

		public virtual IPropertyMapper Set<T>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData)
		{
			return new SetCollectionMapper<T>(_proxyEnversFactory,
														commonCollectionMapperData, 
			                                 typeof (ISet<T>),
			                                 elementComponentData);
		}

		public virtual IPropertyMapper Set(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData)
		{
			throw new NotImplementedException("Non generic set is not supported by DefaultCollectionMapperFactory");
		}

		public virtual IPropertyMapper SortedSet(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, IComparer comparer)
		{
			throw new NotImplementedException("Non generic sorted set is not supported by DefaultCollectionMapperFactory");
		}

		public virtual IPropertyMapper SortedSet<T>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, IComparer<T> comparer)
		{
			return new SortedSetCollectionMapper<T>(_proxyEnversFactory,
											commonCollectionMapperData,
											typeof(ISet<T>),
											elementComponentData,
											comparer);
		}

		public virtual IPropertyMapper List<T>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData)
		{
			return new ListCollectionMapper<T>(_proxyEnversFactory, commonCollectionMapperData, typeof(IList<T>), elementComponentData, indexComponentData);
		}

		public virtual IPropertyMapper List(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData)
		{
			throw new NotImplementedException("Non generic list is not supported by DefaultCollectionMapperFactory");
		}

		public virtual IPropertyMapper Map<TKey, TValue>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData)
		{
			return new MapCollectionMapper<TKey, TValue>(_proxyEnversFactory, commonCollectionMapperData, typeof(IDictionary<TKey, TValue>), elementComponentData, indexComponentData);
		}

		public virtual IPropertyMapper Map(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData)
		{
			throw new NotImplementedException("Non generic map is not supported by DefaultCollectionMapperFactory");
		}

		public virtual IPropertyMapper SortedMap(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, IComparer comparer)
		{
			throw new NotImplementedException("Non generic sorted map is not supported by DefaultCollectionMapperFactory");
		}

		public virtual IPropertyMapper SortedMap<TKey, TValue>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, IComparer<TKey> comparer)
		{
			return new SortedMapCollectionMapper<TKey, TValue>(_proxyEnversFactory, commonCollectionMapperData, typeof(IDictionary<TKey, TValue>), elementComponentData, indexComponentData, comparer);
		}

		public virtual IPropertyMapper Bag<T>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData)
		{
			return new BagCollectionMapper<T>(_proxyEnversFactory, commonCollectionMapperData, typeof(IList<T>), elementComponentData);
		}

		public virtual IPropertyMapper Bag(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData)
		{
			throw new NotImplementedException("Non generic bag is not supported by DefaultCollectionMapperFactory");
		}
	}
}