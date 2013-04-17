using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor
{
	public class SortedSetCollectionInitializor<T> : SetCollectionInitializor<T>
	{
		private readonly IComparer<T> _comparer;

		public SortedSetCollectionInitializor(AuditConfiguration verCfg, 
														IAuditReaderImplementor versionsReader, 
														IRelationQueryGenerator queryGenerator, 
														object primaryKey, 
														long revision, 
														bool removed,
														MiddleComponentData elementComponentData,
														IComparer<T> comparer) : 
				base(verCfg, versionsReader, queryGenerator, primaryKey, revision, removed, elementComponentData)
		{
			_comparer = comparer;
		}

		protected override Iesi.Collections.Generic.ISet<T> InitializeCollection(int size)
		{
			return new Iesi.Collections.Generic.SortedSet<T>(_comparer);
		}
	}
}