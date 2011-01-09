using System;
using System.Collections;
using Iesi.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor
{
	public class SetCollectionInitializor<T> : AbstractCollectionInitializor<ISet<T>>
	{
		private readonly System.Type collectionType;
		private readonly MiddleComponentData elementComponentData;

		public SetCollectionInitializor(AuditConfiguration verCfg,
											IAuditReaderImplementor versionsReader,
											IRelationQueryGenerator queryGenerator,
											Object primaryKey, long revision,
											System.Type collectionType,
											MiddleComponentData elementComponentData) 
								:base(verCfg, versionsReader, queryGenerator, primaryKey, revision)
		{
			this.collectionType = collectionType;
			this.elementComponentData = elementComponentData;
		}

		protected override ISet<T> InitializeCollection(int size) 
		{
			return (ISet<T>) Activator.CreateInstance(collectionType);
		}

		protected override void AddToCollection(ISet<T> collection, object collectionRow) 
		{
			var elementData = ((IList) collectionRow)[elementComponentData.ComponentIndex];

			// If the target entity is not audited, the elements may be the entities already, so we have to check
			// if they are maps or not.
			T element;

			//rk - have a look at this later. ugly!
			//also - investigate: will change tracking work when data is wrapped?
			var elementDataAsDic = elementData as IDictionary;
			if (elementDataAsDic!=null) 
			{
				element = (T)elementComponentData.ComponentMapper.MapToObjectFromFullMap(entityInstantiator, DictionaryWrapper<string, object>.Wrap(elementDataAsDic), null, revision);
			} 
			else 
			{
				element = (T)elementData;
			}
			collection.Add(element);
		}
	}
}
