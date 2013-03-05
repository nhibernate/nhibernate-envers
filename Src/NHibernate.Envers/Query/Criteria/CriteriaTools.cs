using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Query.Property;
using NHibernate.Envers.Reader;

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

		public static string DeterminePropertyName(AuditConfiguration auditCfg, IAuditReaderImplementor versionsReader,
		                                           string entityName, IPropertyNameGetter propertyNameGetter)
		{
			return DeterminePropertyName(auditCfg, versionsReader, entityName, propertyNameGetter.Get(auditCfg));
		}

		public static string DeterminePropertyName(AuditConfiguration auditCfg, IAuditReaderImplementor versionsReader,
		                                           string entityName, string propertyName)
		{
			if (AuditId.IdentifierPlaceholder.Equals(propertyName))
			{
				var identifierPropertyName =
					versionsReader.SessionImplementor.Factory.GetEntityPersister(entityName).IdentifierPropertyName;
				propertyName = auditCfg.AuditEntCfg.OriginalIdPropName + "." + identifierPropertyName;
			}
			return propertyName;
		}
	}
}
