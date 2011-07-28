using System.Collections;
using System.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	public class MapCollectionMapper<K, V> : AbstractCollectionMapper
	{
		private readonly MiddleComponentData _elementComponentData;
		private readonly MiddleComponentData _indexComponentData;

		public MapCollectionMapper(CommonCollectionMapperData commonCollectionMapperData, 
									MiddleComponentData elementComponentData, 
									MiddleComponentData indexComponentData) 
					: base(commonCollectionMapperData, typeof (Dictionary<K, V>), typeof(MapProxy<K, V>))
		{
			_elementComponentData = elementComponentData;
			_indexComponentData = indexComponentData;
		}

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
			var keyValue = (KeyValuePair<K, V>) changed;
			_elementComponentData.ComponentMapper.MapToMapFromObject(data, keyValue.Value);
			_indexComponentData.ComponentMapper.MapToMapFromObject(data, keyValue.Key);
		}

		protected override object GetInitializor(AuditConfiguration verCfg, IAuditReaderImplementor versionsReader, object primaryKey, long revision)
		{
			return new MapCollectionInitializor<K, V>(verCfg, versionsReader, CommonCollectionMapperData.QueryGenerator,
			                                          primaryKey, revision, CollectionType, _elementComponentData,
			                                          _indexComponentData);
		}
	}
}