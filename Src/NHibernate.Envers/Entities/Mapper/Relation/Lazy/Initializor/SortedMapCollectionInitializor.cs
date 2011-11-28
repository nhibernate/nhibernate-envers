using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor
{
	public class SortedMapCollectionInitializor<TKey, TValue> : MapCollectionInitializor<TKey, TValue>
	{
		private readonly IComparer<TKey> _comparer;

		public SortedMapCollectionInitializor(AuditConfiguration verCfg, 
													IAuditReaderImplementor versionsReader, 
													IRelationQueryGenerator queryGenerator, 
													object primaryKey, 
													long revision, 
													MiddleComponentData elementComponentData, 
													MiddleComponentData indexComponentData,
													IComparer<TKey> comparer) 
			: base(verCfg, versionsReader, queryGenerator, primaryKey, revision, elementComponentData, indexComponentData)
		{
			_comparer = comparer;
		}

		protected override IDictionary<TKey, TValue> InitializeCollection(int size)
		{
			return new SortedDictionary<TKey, TValue>(_comparer);
		}
	}
}