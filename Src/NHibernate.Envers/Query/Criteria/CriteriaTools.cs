using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Query.Property;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;

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
			var sessionFactory = versionsReader.SessionImplementor.Factory;
			if (AuditId.IdentifierPlaceholder.Equals(propertyName))
			{
				var identifierPropertyName = sessionFactory.GetEntityPersister(entityName).IdentifierPropertyName;
				return auditCfg.AuditEntCfg.OriginalIdPropName + "." + identifierPropertyName;
			}

			var idPropertyName = sessionFactory.GetEntityPersister(entityName).IdentifierPropertyName;
			if (propertyName.Equals(idPropertyName))
			{
				return auditCfg.AuditEntCfg.OriginalIdPropName + "." + propertyName;
			}
			if (propertyName.StartsWith(idPropertyName + MappingTools.RelationCharacter))
			{
				return auditCfg.AuditEntCfg.OriginalIdPropName + "." + propertyName.Substring(idPropertyName.Length + 1);
			}

			return propertyName;
		}
	}
}
