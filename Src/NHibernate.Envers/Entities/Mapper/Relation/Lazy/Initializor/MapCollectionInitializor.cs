using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor
{
	public class MapCollectionInitializor<TKey, TValue> : AbstractCollectionInitializor<IDictionary<TKey, TValue>>
	{
		private readonly MiddleComponentData _elementComponentData;
		private readonly MiddleComponentData _indexComponentData;

		public MapCollectionInitializor(AuditConfiguration verCfg, 
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

		protected override IDictionary<TKey, TValue> InitializeCollection(int size)
		{
			return new Dictionary<TKey, TValue>(size);
		}

		protected override void AddToCollection(IDictionary<TKey, TValue> collection, object collectionRow)
		{
			var listRow = (IList) collectionRow;
			var elementData = listRow[_elementComponentData.ComponentIndex];
			var indexData = listRow[_indexComponentData.ComponentIndex];

			var element = (TValue)_elementComponentData.ComponentMapper.MapToObjectFromFullMap(EntityInstantiator,
																				(IDictionary) elementData,
																				null,
																				Revision);

			var index = (TKey)_indexComponentData.ComponentMapper.MapToObjectFromFullMap(EntityInstantiator,
																				(IDictionary) indexData,
																				element,
																				Revision);
			collection[index] = element;
		}
	}
}