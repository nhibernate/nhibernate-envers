using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Relation;

namespace NHibernate.Envers.Configuration.Metadata
{
	public interface ICollectionProxyMapperFactory
	{
		IPropertyMapper Set<T>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData);
		IPropertyMapper Set(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData);
		IPropertyMapper List<T>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData);
		IPropertyMapper List(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData);
		IPropertyMapper Map<TKey, TValue>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData);
		IPropertyMapper Map(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, MiddleComponentData indexComponentData);
		IPropertyMapper Bag<T>(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData);
		IPropertyMapper Bag(CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData);
	}
}