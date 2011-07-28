﻿using System.Collections;
using System.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	public class SetCollectionMapper<T> : AbstractCollectionMapper
	{
		public SetCollectionMapper(CommonCollectionMapperData commonCollectionMapperData,
											System.Type collectionType,
											System.Type proxyType,
											MiddleComponentData elementComponentData)
			: base(commonCollectionMapperData, collectionType, proxyType)
		{
			ElementComponentData = elementComponentData;
		}

		protected MiddleComponentData ElementComponentData { get; private set; }

		protected override object GetInitializor(AuditConfiguration verCfg,
													 IAuditReaderImplementor versionsReader,
													 object primaryKey,
													 long revision)
		{
			return new SetCollectionInitializor<T>(verCfg,
										versionsReader,
										CommonCollectionMapperData.QueryGenerator,
										primaryKey,
										revision,
										CollectionType,
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

		protected override void MapToMapFromObject(IDictionary<string, object> data, object changed)
		{
			ElementComponentData.ComponentMapper.MapToMapFromObject(data, changed);
		}
	}
}
