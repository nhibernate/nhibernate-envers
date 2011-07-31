using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Exceptions;

namespace NHibernate.Envers.Query.Criteria
{
	public static class CriteriaTools
	{
		public static void CheckPropertyNotARelation(AuditConfiguration verCfg, string entityName,
												  string propertyName)
		{
			if (verCfg.EntCfg[entityName].IsRelation(propertyName))
			{
				throw new AuditException("This criterion cannot be used on a property that is " +
						  "a relation to another property.");
			}
		}

		public static RelationDescription GetRelatedEntity(AuditConfiguration verCfg, string entityName,
															string propertyName)
		{
			var relationDesc = verCfg.EntCfg.GetRelationDescription(entityName, propertyName);

			if (relationDesc == null)
			{
				return null;
			}

			if (relationDesc.RelationType == RelationType.ToOne)
			{
				return relationDesc;
			}

			throw new AuditException("This type of relation (" + entityName + "." + propertyName +
					  ") isn't supported and can't be used in queries.");
		}
	}
}
