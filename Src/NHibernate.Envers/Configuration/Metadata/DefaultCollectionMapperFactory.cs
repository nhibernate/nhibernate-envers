using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Relation;

namespace NHibernate.Envers.Configuration.Metadata
{
	[Serializable]
	public class DefaultCollectionMapperFactory : ICollectionMapperFactory
	{
		public virtual IPropertyMapper Array(IEnversProxyFactory enversProxyFactory, System.Type elementType, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData)
		{
			throw new NotImplementedException("Array is not supported by DefaultCollectionMapperFactory");
		}

		public virtual IPropertyMapper IdBag<T>(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData)
		{
			return new IdBagCollectionMapper<T>(enversProxyFactory, commonCollectionMapperData, typeof(IList<T>), elementComponentData);
		}

		public virtual IPropertyMapper IdBag(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData)
		{
			throw new NotImplementedException("Non generic idbag is not supported by DefaultCollectionMapperFactory");
		}

		public virtual IPropertyMapper Set<T>(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData)
		{
			return new SetCollectionMapper<T>(enversProxyFactory,
														commonCollectionMapperData, 
			                                 typeof (Iesi.Collections.Generic.ISet<T>),
			                                 elementComponentData);
		}

		public virtual IPropertyMapper Set(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData)
		{
			throw new NotImplementedException("Non generic set is not supported by DefaultCollectionMapperFactory");
		}

		public virtual IPropertyMapper SortedSet(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, IComparer comparer)
		{
			throw new NotImplementedException("Non generic sorted set is not supported by DefaultCollectionMapperFactory");
		}

		public virtual IPropertyMapper SortedSet<T>(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, IComparer<T> comparer)
		{
			return new SortedSetCollectionMapper<T>(enversProxyFactory,
											commonCollectionMapperData,
											typeof(Iesi.Collections.Generic.ISet<T>),
											elementComponentData,
											comparer);
		}

		public virtual IPropertyMapper List<T>(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData)
		{
			return new ListCollectionMapper<T>(enversProxyFactory, commonCollectionMapperData, typeof(IList<T>), elementComponentData, indexComponentData);
		}

		public virtual IPropertyMapper List(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData)
		{
			throw new NotImplementedException("Non generic list is not supported by DefaultCollectionMapperFactory");
		}

		public virtual IPropertyMapper Map<TKey, TValue>(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData)
		{
			return new MapCollectionMapper<TKey, TValue>(enversProxyFactory, commonCollectionMapperData, typeof(IDictionary<TKey, TValue>), elementComponentData, indexComponentData);
		}

		public virtual IPropertyMapper Map(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData)
		{
			throw new NotImplementedException("Non generic map is not supported by DefaultCollectionMapperFactory");
		}

		public virtual IPropertyMapper SortedMap(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, IComparer comparer)
		{
			throw new NotImplementedException("Non generic sorted map is not supported by DefaultCollectionMapperFactory");
		}

		public virtual IPropertyMapper SortedMap<TKey, TValue>(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, IComparer<TKey> comparer)
		{
			return new SortedMapCollectionMapper<TKey, TValue>(enversProxyFactory, commonCollectionMapperData, typeof(IDictionary<TKey, TValue>), elementComponentData, indexComponentData, comparer);
		}

		public virtual IPropertyMapper Bag<T>(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData)
		{
			return new BagCollectionMapper<T>(enversProxyFactory, commonCollectionMapperData, typeof(IList<T>), elementComponentData);
		}

		public virtual IPropertyMapper Bag(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData)
		{
			throw new NotImplementedException("Non generic bag is not supported by DefaultCollectionMapperFactory");
		}
	}
}