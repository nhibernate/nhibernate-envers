using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor
{
	public abstract class AbstractCollectionInitializor<T> : IInitializor<T>
	{
		private readonly IAuditReaderImplementor versionsReader;
		private readonly IRelationQueryGenerator queryGenerator;
		private readonly object primaryKey;

		protected AbstractCollectionInitializor(AuditConfiguration verCfg,
												IAuditReaderImplementor versionsReader,
												IRelationQueryGenerator queryGenerator,
												object primaryKey, 
												long revision) 
		{
			this.versionsReader = versionsReader;
			this.queryGenerator = queryGenerator;
			this.primaryKey = primaryKey;
			Revision = revision;
			EntityInstantiator = new EntityInstantiator(verCfg, versionsReader);
		}

		protected EntityInstantiator EntityInstantiator { get; private set; }
		protected long Revision { get; private set; }

		protected abstract T InitializeCollection(int size);

		protected abstract void AddToCollection(T collection, object collectionRow);

		public T Initialize() 
		{
			var collectionContent = queryGenerator.GetQuery(versionsReader, primaryKey, Revision).List();
			var collection = InitializeCollection(collectionContent.Count);
			foreach (var collectionRow in collectionContent) 
			{
				AddToCollection(collection, collectionRow);
			}
			return collection;
		}
	}
}
