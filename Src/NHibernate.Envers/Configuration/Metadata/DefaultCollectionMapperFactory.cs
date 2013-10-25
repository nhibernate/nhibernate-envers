using System;
using System.Collections.Generic;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Relation;

namespace NHibernate.Envers.Configuration.Metadata
{
	[Serializable]
	public class DefaultCollectionMapperFactory : ICollectionMapperFactory
	{
		public virtual IPropertyMapper Array(IEnversProxyFactory enversProxyFactory, System.Type elementType, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, bool embeddableElementType)
		{
			throw new NotImplementedException("Array is not supported by DefaultCollectionMapperFactory");
		}

		public virtual IPropertyMapper IdBag<T>(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, bool embeddableElementType)
		{
			return new IdBagCollectionMapper<T>(enversProxyFactory, commonCollectionMapperData, typeof(IList<T>), elementComponentData, embeddableElementType);
		}

		public virtual IPropertyMapper Set<T>(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, bool embeddableElementType)
		{
			return new SetCollectionMapper<T>(enversProxyFactory,
			                                  commonCollectionMapperData,
			                                  typeof (ISet<T>),
			                                  elementComponentData,
			                                  embeddableElementType,
			                                  embeddableElementType);
		}

		public virtual IPropertyMapper SortedSet<T>(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, IComparer<T> comparer, bool embeddableElementType)
		{
			return new SortedSetCollectionMapper<T>(enversProxyFactory,
											commonCollectionMapperData,
											typeof(ISet<T>),
											elementComponentData,
											comparer, 
											embeddableElementType,
											embeddableElementType);
		}

		public virtual IPropertyMapper List<T>(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, bool embeddableElementType)
		{
			return new ListCollectionMapper<T>(enversProxyFactory, commonCollectionMapperData, typeof(IList<T>), elementComponentData, indexComponentData, embeddableElementType);
		}

		public virtual IPropertyMapper Map<TKey, TValue>(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, bool embeddableElementType)
		{
			return new MapCollectionMapper<TKey, TValue>(enversProxyFactory, commonCollectionMapperData, typeof(IDictionary<TKey, TValue>), elementComponentData, indexComponentData, embeddableElementType);
		}

		public virtual IPropertyMapper SortedMap<TKey, TValue>(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, IComparer<TKey> comparer, bool embeddableElementType)
		{
			return new SortedMapCollectionMapper<TKey, TValue>(enversProxyFactory, commonCollectionMapperData, typeof(IDictionary<TKey, TValue>), elementComponentData, indexComponentData, comparer, embeddableElementType);
		}

		public virtual IPropertyMapper Bag<T>(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, bool embeddableElementType)
		{
			return new BagCollectionMapper<T>(enversProxyFactory, commonCollectionMapperData, typeof(IList<T>), elementComponentData, embeddableElementType);
		}
	}
}