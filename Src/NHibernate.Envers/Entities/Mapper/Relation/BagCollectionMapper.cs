using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Collection;
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
									MiddleComponentData elementComponentData)
			: base(enversProxyFactory, commonCollectionMapperData, proxyType)
		{    
			_elementComponentData = elementComponentData;
		}

		protected override IInitializor GetInitializor(AuditConfiguration verCfg, 
														IAuditReaderImplementor versionsReader,
														object primaryKey, 
														long revision) 
		{
			return new BagCollectionInitializor<T>(verCfg, 
											versionsReader, 
											CommonCollectionMapperData.QueryGenerator,
											primaryKey, 
											revision,
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

		protected override void MapToMapFromObject(IDictionary<string, object> data, object changed) 
		{
			_elementComponentData.ComponentMapper.MapToMapFromObject(data, changed);
		}
	}
}