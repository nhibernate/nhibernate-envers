using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Relation;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.CustomMapping.UserCollection
{
	public class CustomCollectionFactory : ICustomCollectionFactory
	{
		public IPropertyMapper Create(ICollectionProxyFactory collectionProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData)
		{
			return new SpecialMapper(collectionProxyFactory, commonCollectionMapperData, elementComponentData);
		}
	}
}