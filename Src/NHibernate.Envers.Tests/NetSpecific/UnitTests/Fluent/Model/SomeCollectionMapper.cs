using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Relation;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent.Model
{
	public class SomeCollectionMapper : ICustomCollectionMapperFactory
	{
		public IPropertyMapper Create(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData, bool embeddableElementType)
		{
			return null;
		}
	}
}