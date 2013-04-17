using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	[Serializable]
	public class BagCollectionMapper<T> : AbstractCollectionMapper
	{
		private readonly MiddleComponentData _elementComponentData;

		public BagCollectionMapper(IEnversProxyFactory enversProxyFactory, 
									CommonCollectionMapperData commonCollectionMapperData,
									System.Type proxyType,
									MiddleComponentData elementComponentData, 
									bool revisionTypeInId)
			: base(enversProxyFactory, commonCollectionMapperData, proxyType, false, revisionTypeInId)
		{    
			_elementComponentData = elementComponentData;
		}

		protected override IInitializor GetInitializor(AuditConfiguration verCfg, 
														IAuditReaderImplementor versionsReader,
														object primaryKey, 
														long revision,
														bool removed) 
		{
			return new BagCollectionInitializor<T>(verCfg, 
											versionsReader, 
											CommonCollectionMapperData.QueryGenerator,
											primaryKey, 
											revision,
											removed,
											_elementComponentData);
		}

		protected override IEnumerable GetNewCollectionContent(IPersistentCollection newCollection) 
		{
			return  (ICollection) newCollection;
		}

		protected override IEnumerable GetOldCollectionContent(object oldCollection)
		{
			if (oldCollection == null) 
			{
				return null;
			}
			var oldCollAsDic = oldCollection as IDictionary;
			if (oldCollAsDic != null)
			{
				return oldCollAsDic.Keys;
			}
			return (IEnumerable) oldCollection;
		}

		protected override void MapToMapFromObject(ISessionImplementor session, IDictionary<string, object> idData, IDictionary<string, object> data, object changed)
		{
			_elementComponentData.ComponentMapper.MapToMapFromObject(session, idData, data, changed);
		}
	}
}