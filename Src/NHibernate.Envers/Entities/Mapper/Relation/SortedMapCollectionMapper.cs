using System.Collections.Generic;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	public class SortedMapCollectionMapper<TKey, TValue> : MapCollectionMapper<TKey, TValue>
	{
		private readonly IComparer<TKey> _comparer;

		public SortedMapCollectionMapper(CommonCollectionMapperData commonCollectionMapperData, 
													System.Type proxyType, 
													MiddleComponentData elementComponentData, 
													MiddleComponentData indexComponentData,
													IComparer<TKey> comparer) 
			: base(commonCollectionMapperData, proxyType, elementComponentData, indexComponentData)
		{
			_comparer = comparer;
		}

		protected override object GetInitializor(Configuration.AuditConfiguration verCfg, Reader.IAuditReaderImplementor versionsReader, object primaryKey, long revision)
		{
			return new SortedMapCollectionInitializor<TKey, TValue>(verCfg, versionsReader, CommonCollectionMapperData.QueryGenerator,
																	primaryKey, revision, ElementComponentData,
																	IndexComponentData, _comparer);
		}
	}
}