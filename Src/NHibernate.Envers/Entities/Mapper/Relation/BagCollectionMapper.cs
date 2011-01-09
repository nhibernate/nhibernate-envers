using System.Collections;
using System.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	public class BagCollectionMapper<T> : AbstractCollectionMapper<IList<T>>
	{
		private readonly MiddleComponentData elementComponentData;

		public BagCollectionMapper(CommonCollectionMapperData commonCollectionMapperData,
									MiddleComponentData elementComponentData)
			: base(commonCollectionMapperData, typeof(List<>), typeof(ListProxy<T>))
		{    
			this.elementComponentData = elementComponentData;
		}

		protected override IInitializor<IList<T>> GetInitializor(AuditConfiguration verCfg, 
														IAuditReaderImplementor versionsReader,
														object primaryKey, 
														long revision) 
		{
			return new BagCollectionInitializor<T>(verCfg, 
											versionsReader, 
											commonCollectionMapperData.QueryGenerator,
											primaryKey, 
											revision,
											elementComponentData);
		}

		protected override IEnumerable GetNewCollectionContent(IPersistentCollection newCollection) 
		{
			return  (ICollection) newCollection;
		}

		protected override IEnumerable GetOldCollectionContent(object oldCollection)
		{
			if (oldCollection == null) 
			{
				return null;
			}
			var oldCollAsDic = oldCollection as IDictionary;
			if (oldCollAsDic != null)
			{
				return oldCollAsDic.Keys;
			}
			return (IEnumerable) oldCollection;
		}

		protected override void MapToMapFromObject(IDictionary<string, object> data, object changed) 
		{
			elementComponentData.ComponentMapper.MapToMapFromObject(data, changed);
		}
	}
}