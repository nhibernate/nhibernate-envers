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

		public AuditEntitiesConfiguration VerEntCfg { get; }
		public string VersionsMiddleEntityName { get; }
		public PropertyData CollectionReferencingPropertyData { get; }
		public MiddleIdData ReferencingIdData { get; }
		public IRelationQueryGenerator QueryGenerator { get; }
	}
}
