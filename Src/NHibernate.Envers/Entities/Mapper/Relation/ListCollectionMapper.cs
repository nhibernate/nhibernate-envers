using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	[Serializable]
	public class ListCollectionMapper<T> : AbstractCollectionMapper
	{
		private readonly MiddleComponentData _elementComponentData;
		private readonly MiddleComponentData _indexComponentData;

		public ListCollectionMapper(IEnversProxyFactory enversProxyFactory,
									CommonCollectionMapperData commonCollectionMapperData,
									System.Type proxyType,
									MiddleComponentData elementComponentData,
									MiddleComponentData indexComponentData,
									bool revisionTypeInId) 
						: base(enversProxyFactory, commonCollectionMapperData, proxyType, revisionTypeInId)
		{
			_elementComponentData = elementComponentData;
			_indexComponentData = indexComponentData;
		}

		protected override IEnumerable GetNewCollectionContent(IPersistentCollection newCollection)
		{
			return newCollection == null ? null : Toolz.ListToIndexElementPairList<T>((IList)newCollection);
		}

		protected override IEnumerable GetOldCollectionContent(object oldCollection)
		{
			return oldCollection == null ? null : Toolz.ListToIndexElementPairList<T>((IList)oldCollection);
		}

		protected override void MapToMapFromObject(ISessionImplementor session, IDictionary<String, Object> idData, IDictionary<string, object> data, object changed)
		{
			var indexValuePair = (Pair<int, T>)changed;
			_elementComponentData.ComponentMapper.MapToMapFromObject(session, idData, data, indexValuePair.Second);
			_indexComponentData.ComponentMapper.MapToMapFromObject(session, idData, data, indexValuePair.First);
		}

		protected override IInitializor GetInitializor(AuditConfiguration verCfg, IAuditReaderImplementor versionsReader, object primaryKey, long revision)
		{
			return new ListCollectionInitializor<T>(verCfg, 
												versionsReader, 
												CommonCollectionMapperData.QueryGenerator,
												primaryKey, 
												revision, 
												_elementComponentData, 
												_indexComponentData);
		}
	}
}