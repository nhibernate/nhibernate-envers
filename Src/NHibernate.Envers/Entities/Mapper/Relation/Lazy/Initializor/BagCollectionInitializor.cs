using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor
{
	public class BagCollectionInitializor<T> : AbstractCollectionInitializor<IList<T>>
	{
		private readonly MiddleComponentData _elementComponentData;

		public BagCollectionInitializor(AuditConfiguration verCfg,
											IAuditReaderImplementor versionsReader,
											IRelationQueryGenerator queryGenerator,
											object primaryKey, 
											long revision,
											bool removed,
											MiddleComponentData elementComponentData) 
								:base(verCfg, versionsReader, queryGenerator, primaryKey, revision, removed)
		{
			_elementComponentData = elementComponentData;
		}

		protected override IList<T> InitializeCollection(int size) 
		{
			return new List<T>(size);
		}

		protected override void AddToCollection(IList<T> collection, object collectionRow) 
		{
			var elementData = ((IList) collectionRow)[_elementComponentData.ComponentIndex];

			// If the target entity is not audited, the elements may be the entities already, so we have to check
			// if they are maps or not.
			T element;

			//rk - have a look at this later. ugly!
			//also - investigate: will change tracking work when data is wrapped?
			var elementDataAsDic = elementData as IDictionary;
			if (elementDataAsDic!=null) 
			{
				element = (T)_elementComponentData.ComponentMapper.MapToObjectFromFullMap(EntityInstantiator, elementDataAsDic, null, Revision);
			} 
			else 
			{
				element = (T)elementData;
			}
			collection.Add(element);
		}
	}
}