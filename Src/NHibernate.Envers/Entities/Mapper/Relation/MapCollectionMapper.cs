using System.Collections;
using System.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	public class MapCollectionMapper<TKey, TValue> : AbstractCollectionMapper
	{
		public MapCollectionMapper(ICollectionProxyFactory collectionProxyFactory,
											CommonCollectionMapperData commonCollectionMapperData,
											System.Type proxyType,
											MiddleComponentData elementComponentData, 
											MiddleComponentData indexComponentData) 
					: base(collectionProxyFactory, commonCollectionMapperData, proxyType)
		{
			ElementComponentData = elementComponentData;
			IndexComponentData = indexComponentData;
		}

		protected MiddleComponentData ElementComponentData { get; private set; }
		protected MiddleComponentData IndexComponentData { get; private set; }

		protected override IEnumerable GetNewCollectionContent(IPersistentCollection newCollection)
		{
			return newCollection == null ? null : (IEnumerable) newCollection;
		}

		protected override IEnumerable GetOldCollectionContent(object oldCollection)
		{
			return oldCollection == null ? null : (IEnumerable) oldCollection;
		}

		protected override void MapToMapFromObject(IDictionary<string, object> data, object changed)
		{
			var keyValue = (KeyValuePair<TKey, TValue>) changed;
			ElementComponentData.ComponentMapper.MapToMapFromObject(data, keyValue.Value);
			IndexComponentData.ComponentMapper.MapToMapFromObject(data, keyValue.Key);
		}

		protected override IInitializor GetInitializor(AuditConfiguration verCfg, IAuditReaderImplementor versionsReader, object primaryKey, long revision)
		{
			return new MapCollectionInitializor<TKey, TValue>(verCfg, versionsReader, CommonCollectionMapperData.QueryGenerator,
			                                          primaryKey, revision, ElementComponentData,
			                                          IndexComponentData);
		}
	}
}