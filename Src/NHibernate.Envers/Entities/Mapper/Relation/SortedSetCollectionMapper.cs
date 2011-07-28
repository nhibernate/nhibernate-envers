using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	public class SortedSetCollectionMapper<T> : SetCollectionMapper<T>
	{
		private readonly IComparer<T> _comparer;

		public SortedSetCollectionMapper(CommonCollectionMapperData commonCollectionMapperData, 
													System.Type collectionType, 
													System.Type proxyType, 
													MiddleComponentData elementComponentData,
													IComparer<T> comparer) 
			: base(commonCollectionMapperData, collectionType, proxyType, elementComponentData)
		{
			_comparer = comparer;
		}

		protected override object GetInitializor(AuditConfiguration verCfg, IAuditReaderImplementor versionsReader, object primaryKey, long revision)
		{
			return new SortedSetCollectionInitializor<T>(verCfg, 
																	versionsReader, 
																	CommonCollectionMapperData.QueryGenerator,
																	primaryKey, 
																	revision, 
																	CollectionType, 
																	ElementComponentData, 
																	_comparer);
		}
	}
}