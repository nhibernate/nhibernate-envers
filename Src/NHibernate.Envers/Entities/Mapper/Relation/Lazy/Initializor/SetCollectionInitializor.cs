using System;
using System.Collections;
using Iesi.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor
{
	public class SetCollectionInitializor<T> : AbstractCollectionInitializor<ISet<T>>
	{
		private readonly MiddleComponentData elementComponentData;

		public SetCollectionInitializor(AuditConfiguration verCfg,
											IAuditReaderImplementor versionsReader,
											IRelationQueryGenerator queryGenerator,
											object primaryKey, long revision,
											System.Type collectionType,
											MiddleComponentData elementComponentData) 
								:base(verCfg, versionsReader, queryGenerator, primaryKey, revision)
		{
			CollectionType = collectionType;
			this.elementComponentData = elementComponentData;
		}

		protected System.Type CollectionType { get; private set; }

		protected override ISet<T> InitializeCollection(int size) 
		{
			return (ISet<T>) Activator.CreateInstance(CollectionType);
		}

		protected override void AddToCollection(ISet<T> collection, object collectionRow) 
		{
			var elementData = ((IList) collectionRow)[elementComponentData.ComponentIndex];

			// If the target entity is not audited, the elements may be the entities already, so we have to check
			// if they are maps or not.
			T element;

			var elementDataAsDic = elementData as IDictionary;
			if (elementDataAsDic!=null) 
			{
				element = (T)elementComponentData.ComponentMapper.MapToObjectFromFullMap(EntityInstantiator, elementDataAsDic, null, Revision);
			} 
			else 
			{
				element = (T)elementData;
			}
			collection.Add(element);
		}
	}
}
