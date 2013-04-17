using System;
using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	[Serializable]
	public class SortedSetCollectionMapper<T> : SetCollectionMapper<T>
	{
		private readonly IComparer<T> _comparer;

		public SortedSetCollectionMapper(IEnversProxyFactory enversProxyFactory, 
													CommonCollectionMapperData commonCollectionMapperData, 
													System.Type proxyType, 
													MiddleComponentData elementComponentData,
													IComparer<T> comparer,
													bool ordinalInId,
													bool revisionTypeInId)
			: base(enversProxyFactory, commonCollectionMapperData, proxyType, elementComponentData, ordinalInId, revisionTypeInId)
		{
			_comparer = comparer;
		}

		protected override IInitializor GetInitializor(AuditConfiguration verCfg, IAuditReaderImplementor versionsReader, object primaryKey, long revision, bool removed)
		{
			return new SortedSetCollectionInitializor<T>(verCfg, 
																	versionsReader, 
																	CommonCollectionMapperData.QueryGenerator,
																	primaryKey, 
																	revision,
																	removed,
																	ElementComponentData, 
																	_comparer);
		}
	}
}