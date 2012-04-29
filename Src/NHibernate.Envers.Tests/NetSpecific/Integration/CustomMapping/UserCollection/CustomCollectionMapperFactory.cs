﻿using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Relation;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.CustomMapping.UserCollection
{
	public class CustomCollectionMapperFactory : ICustomCollectionMapperFactory
	{
		public IPropertyMapper Create(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData)
		{
			return new SpecialMapper(enversProxyFactory, commonCollectionMapperData, elementComponentData);
		}
	}
}