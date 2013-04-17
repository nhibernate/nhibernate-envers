using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor
{
	public class ListCollectionInitializor<T> : AbstractCollectionInitializor<IList<T>>
	{
		private readonly MiddleComponentData _elementComponentData;
		private readonly MiddleComponentData _indexComponentData;

		public ListCollectionInitializor(AuditConfiguration verCfg, 
											IAuditReaderImplementor versionsReader, 
											IRelationQueryGenerator queryGenerator, 
											object primaryKey, 
											long revision,
											bool removed,
											MiddleComponentData elementComponentData,
											MiddleComponentData indexComponentData) 
								: base(verCfg, versionsReader, queryGenerator, primaryKey, revision, removed)
		{
			_elementComponentData = elementComponentData;
			_indexComponentData = indexComponentData;
		}

		protected override IList<T> InitializeCollection(int size)
		{
			var ret = new List<T>(size);
			for (var i = 0; i < size; i++)
			{
				ret.Add(default(T));
			}
			return ret;
		}

		protected override void AddToCollection(IList<T> collection, object collectionRow)
		{
			var listRow = (IList)collectionRow;
			var elementData = listRow[_elementComponentData.ComponentIndex];
			var indexData = listRow[_indexComponentData.ComponentIndex];

			var elementDataAsDic = elementData as IDictionary;
			var element = elementDataAsDic == null
				          ? (T) elementData
				          : (T) _elementComponentData.ComponentMapper.MapToObjectFromFullMap(EntityInstantiator,
				                                                                             (IDictionary) elementData,
				                                                                             null,
				                                                                             Revision);
			var index = (int)_indexComponentData.ComponentMapper.MapToObjectFromFullMap(EntityInstantiator,
																				(IDictionary) indexData, 
																				element, 
																				Revision);
			collection[index] = element;
		}
	}
}