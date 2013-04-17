using System;
using System.Collections.Generic;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	[Serializable]
	public class SortedMapCollectionMapper<TKey, TValue> : MapCollectionMapper<TKey, TValue>
	{
		private readonly IComparer<TKey> _comparer;

		public SortedMapCollectionMapper(IEnversProxyFactory enversProxyFactory,
													CommonCollectionMapperData commonCollectionMapperData, 
													System.Type proxyType, 
													MiddleComponentData elementComponentData, 
													MiddleComponentData indexComponentData,
													IComparer<TKey> comparer,
													bool revisionTypeInId) 
			: base(enversProxyFactory, commonCollectionMapperData, proxyType, elementComponentData, indexComponentData, revisionTypeInId)
		{
			_comparer = comparer;
		}

		protected override IInitializor GetInitializor(Configuration.AuditConfiguration verCfg, Reader.IAuditReaderImplementor versionsReader, object primaryKey, long revision, bool removed)
		{
			return new SortedMapCollectionInitializor<TKey, TValue>(verCfg, versionsReader, CommonCollectionMapperData.QueryGenerator,
																	primaryKey, revision, removed, ElementComponentData,
																	IndexComponentData, _comparer);
		}
	}
}