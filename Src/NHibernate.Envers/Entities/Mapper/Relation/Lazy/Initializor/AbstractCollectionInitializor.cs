using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor
{
	public abstract partial class AbstractCollectionInitializor<T> : IInitializor
	{
		private readonly IAuditReaderImplementor _versionsReader;
		private readonly IRelationQueryGenerator _queryGenerator;
		private readonly object _primaryKey;
		private readonly bool _removed;

		protected AbstractCollectionInitializor(AuditConfiguration verCfg,
												IAuditReaderImplementor versionsReader,
												IRelationQueryGenerator queryGenerator,
												object primaryKey, 
												long revision, 
												bool removed) 
		{
			_versionsReader = versionsReader;
			_queryGenerator = queryGenerator;
			_primaryKey = primaryKey;
			_removed = removed;
			Revision = revision;
			EntityInstantiator = new EntityInstantiator(verCfg, versionsReader);
		}

		protected EntityInstantiator EntityInstantiator { get; }
		protected long Revision { get; }

		protected abstract T InitializeCollection(int size);

		protected abstract void AddToCollection(T collection, object collectionRow);

		public object Initialize() 
		{
			var collectionContent = _queryGenerator.GetQuery(_versionsReader, _primaryKey, Revision, _removed).List();
			var collection = InitializeCollection(collectionContent.Count);
			foreach (var collectionRow in collectionContent) 
			{
				AddToCollection(collection, collectionRow);
			}
			return collection;
		}
	}
}
