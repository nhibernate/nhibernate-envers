using System.Collections;
using System.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	public class ListCollectionMapper<T> : AbstractCollectionMapper<IList<T>> 
	{
		private readonly MiddleComponentData _elementComponentData;
		private readonly MiddleComponentData _indexComponentData;

		public ListCollectionMapper(CommonCollectionMapperData commonCollectionMapperData,
									MiddleComponentData elementComponentData, 
									MiddleComponentData indexComponentData) 
						: base(commonCollectionMapperData, typeof(List<>), typeof(ListProxy<T>))
		{
			_elementComponentData = elementComponentData;
			_indexComponentData = indexComponentData;
		}

		protected override IEnumerable GetNewCollectionContent(IPersistentCollection newCollection)
		{
			return newCollection == null ? null : Toolz.ListToIndexElementPairList<T>((IList)newCollection);
		}

		protected override IEnumerable GetOldCollectionContent(object oldCollection)
		{
			return oldCollection == null ? null : Toolz.ListToIndexElementPairList<T>((IList)oldCollection);
		}

		protected override void MapToMapFromObject(IDictionary<string, object> data, object changed)
		{
			var indexValuePair = (Pair<int, T>)changed;
			_elementComponentData.ComponentMapper.MapToMapFromObject(data, indexValuePair.Second);
			_indexComponentData.ComponentMapper.MapToMapFromObject(data, indexValuePair.First);
		}

		protected override IInitializor<IList<T>> GetInitializor(AuditConfiguration verCfg, IAuditReaderImplementor versionsReader, object primaryKey, long revision)
		{
			return new ListCollectionInitializor<T>(verCfg, 
												versionsReader, 
												commonCollectionMapperData.QueryGenerator,
												primaryKey, 
												revision, 
												_elementComponentData, 
												_indexComponentData);
		}
	}
}