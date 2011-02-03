using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor
{
	public class MapCollectionInitializor<K, V> : AbstractCollectionInitializor<IDictionary<K, V>>
	{
		private readonly System.Type _collectionType;
		private readonly MiddleComponentData _elementComponentData;
		private readonly MiddleComponentData _indexComponentData;

		public MapCollectionInitializor(AuditConfiguration verCfg, 
										IAuditReaderImplementor versionsReader, 
										IRelationQueryGenerator queryGenerator, 
										object primaryKey, 
										long revision,
										System.Type collectionType,
										MiddleComponentData elementComponentData,
										MiddleComponentData indexComponentData) 
						: base(verCfg, versionsReader, queryGenerator, primaryKey, revision)
		{
			_collectionType = collectionType;
			_elementComponentData = elementComponentData;
			_indexComponentData = indexComponentData;
		}

		protected override IDictionary<K, V> InitializeCollection(int size)
		{
			return (IDictionary<K, V>) Activator.CreateInstance(_collectionType);
		}

		protected override void AddToCollection(IDictionary<K, V> collection, object collectionRow)
		{
			var elementData = ((IList)collectionRow)[_elementComponentData.ComponentIndex];
			var indexData = ((IList)collectionRow)[_indexComponentData.ComponentIndex];

			var element = (V)_elementComponentData.ComponentMapper.MapToObjectFromFullMap(entityInstantiator,
																				(IDictionary) elementData,
																				null,
																				revision);

			var index = (K)_indexComponentData.ComponentMapper.MapToObjectFromFullMap(entityInstantiator,
																				(IDictionary) indexData,
																				element,
																				revision);
			collection[index] = element;
		}
	}
}