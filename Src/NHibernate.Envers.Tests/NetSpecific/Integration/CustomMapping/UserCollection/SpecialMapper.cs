using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.CustomMapping.UserCollection
{
	public class SpecialMapper : BagCollectionMapper<Number>
	{
		private readonly MiddleComponentData _elementComponentData;

		public SpecialMapper(IEnversProxyFactory enversProxyFactory, CommonCollectionMapperData commonCollectionMapperData, MiddleComponentData elementComponentData, bool embeddableElementType)
			: base(enversProxyFactory, commonCollectionMapperData, typeof(ISpecialCollection), elementComponentData, embeddableElementType)
		{
			_elementComponentData = elementComponentData;
		}

		protected override IInitializor GetInitializor(AuditConfiguration verCfg, IAuditReaderImplementor versionsReader, object primaryKey, long revision, bool removed)
		{
			return new SpecialInitializor(verCfg,
			                              versionsReader,
			                              CommonCollectionMapperData.QueryGenerator,
			                              primaryKey,
			                              revision,
																		removed,
			                              _elementComponentData);
		}
	}
}