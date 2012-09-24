using System;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	/// <summary>
	/// Data that is used by all collection mappers, regardless of the type.  
	/// </summary>
	[Serializable]
	public sealed class CommonCollectionMapperData
	{
		public CommonCollectionMapperData(AuditEntitiesConfiguration verEntCfg, string versionsMiddleEntityName,
														PropertyData collectionReferencingPropertyData, MiddleIdData referencingIdData,
														IRelationQueryGenerator queryGenerator)
		{
			VerEntCfg = verEntCfg;
			VersionsMiddleEntityName = versionsMiddleEntityName;
			CollectionReferencingPropertyData = collectionReferencingPropertyData;
			ReferencingIdData = referencingIdData;
			QueryGenerator = queryGenerator;
		}

		public AuditEntitiesConfiguration VerEntCfg { get; private set; }
		public string VersionsMiddleEntityName { get; private set; }
		public PropertyData CollectionReferencingPropertyData { get; private set; }
		public MiddleIdData ReferencingIdData { get; private set; }
		public IRelationQueryGenerator QueryGenerator { get; private set; }
	}
}
