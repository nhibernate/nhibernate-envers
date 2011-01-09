using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;

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
											MiddleComponentData elementComponentData,
											MiddleComponentData indexComponentData) 
								: base(verCfg, versionsReader, queryGenerator, primaryKey, revision)
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
			var elementData = ((IList)collectionRow)[_elementComponentData.ComponentIndex];
			var indexData = ((IList)collectionRow)[_indexComponentData.ComponentIndex];

			//rk - have a look at this later. 
			var wrappedElementData = DictionaryWrapper<string, object>.Wrap((IDictionary)elementData);
			var wrappedIndexData = DictionaryWrapper<string, object>.Wrap((IDictionary)indexData);
			var element = (T)_elementComponentData.ComponentMapper.MapToObjectFromFullMap(entityInstantiator,
																				wrappedElementData, 
																				null, 
																				revision);

			var index = (int)_indexComponentData.ComponentMapper.MapToObjectFromFullMap(entityInstantiator,
																				wrappedIndexData, 
																				element, 
																				revision);
			collection[index] = element;
		}
	}
}