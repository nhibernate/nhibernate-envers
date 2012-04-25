using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.CustomMapping.UserCollection
{
	public class SpecialMapper : BagCollectionMapper<Number>
	{
		private readonly MiddleComponentData _elementComponentData;

		public SpecialMapper(ICollectionProxyFactory collectionProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData) : base(collectionProxyFactory, commonCollectionMapperData, typeof(ISpecialCollection), elementComponentData)
		{
			_elementComponentData = elementComponentData;
		}

		protected override IInitializor GetInitializor(AuditConfiguration verCfg, IAuditReaderImplementor versionsReader, object primaryKey, long revision)
		{
			return new SpecialInitializor(verCfg,
			                              versionsReader,
			                              CommonCollectionMapperData.QueryGenerator,
			                              primaryKey,
			                              revision,
			                              _elementComponentData);
		}
	}
}