using System;
using Iesi.Collections.Generic;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Relation;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy;

namespace NHibernate.Envers.Configuration.Metadata
{
	public class DefaultCollectionProxyMapperFactory : ICollectionProxyMapperFactory
	{
		public virtual IPropertyMapper Set<T>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData)
		{
			return new SetCollectionMapper<T>(commonCollectionMapperData, 
			                                  typeof (HashedSet<T>), 
			                                  typeof (SetProxy<T>),
			                                  elementComponentData);
		}

		public virtual IPropertyMapper Set(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData)
		{
			throw new NotImplementedException("Non generic set is not supported by DefaultCollectionProxyMapperFactory");
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
			return new MapCollectionMapper<TKey, TValue>(commonCollectionMapperData, elementComponentData, indexComponentData);
		}

		public virtual IPropertyMapper Map(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData)
		{
			throw new NotImplementedException("Non generic map is not supported by DefaultCollectionProxyMapperFactory");
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