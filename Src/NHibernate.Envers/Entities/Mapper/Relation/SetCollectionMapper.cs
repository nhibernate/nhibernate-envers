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
	public class SetCollectionMapper<T> : AbstractCollectionMapper
	{
		public SetCollectionMapper(IEnversProxyFactory enversProxyFactory, 
											CommonCollectionMapperData commonCollectionMapperData,
											System.Type proxyType,
											MiddleComponentData elementComponentData,
											bool revisionTypeInId)
			: base(enversProxyFactory, commonCollectionMapperData, proxyType, revisionTypeInId)
		{
			ElementComponentData = elementComponentData;
		}

		protected MiddleComponentData ElementComponentData { get; private set; }

		protected override IInitializor GetInitializor(AuditConfiguration verCfg,
													 IAuditReaderImplementor versionsReader,
													 object primaryKey,
													 long revision)
		{
			return new SetCollectionInitializor<T>(verCfg,
										versionsReader,
										CommonCollectionMapperData.QueryGenerator,
										primaryKey,
										revision,
										ElementComponentData);
		}

		protected override IEnumerable GetNewCollectionContent(IPersistentCollection newCollection)
		{
			return (ICollection)newCollection;
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
			return (IEnumerable)oldCollection;
		}

		protected override void MapToMapFromObject(ISessionImplementor session, IDictionary<String, Object> idData, IDictionary<string, object> data, object changed)
		{
			ElementComponentData.ComponentMapper.MapToMapFromObject(session, idData, data, changed);
		}
	}
}
