using System;
using System.Collections;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	[Serializable]
	public abstract partial class AbstractOneToOneMapper : AbstractToOneMapper
	{
		private readonly string _entityName;
		private readonly string _referencedEntityName;

		protected AbstractOneToOneMapper(string entityName, string referencedEntityName, PropertyData propertyData)
			: base(propertyData)
		{
			_entityName = entityName;
			_referencedEntityName = referencedEntityName;
		}

		protected override void NullSafeMapToEntityFromMap(AuditConfiguration verCfg, object obj, IDictionary data, object primaryKey, IAuditReaderImplementor versionsReader, long revision)
		{
			var referencedEntity = GetEntityInfo(verCfg, _referencedEntityName);
			var removed = RevisionType.Deleted.Equals(data[verCfg.AuditEntCfg.RevisionTypePropName]);
			object value;
			try
			{
				value = QueryForReferencedEntity(versionsReader, referencedEntity, primaryKey, revision, removed);
			}
			catch (NonUniqueResultException e)
			{
				throw new AuditException("Many versions results for one-to-one relationship " + _entityName + "." + PropertyData.BeanName + ".", e);
			}

			SetPropertyValue(obj, value);
		}

		protected abstract Object QueryForReferencedEntity(IAuditReaderImplementor versionsReader, EntityInfo referencedEntity, object primaryKey, long revision, bool removed);

		public override void MapModifiedFlagsToMapFromEntity(Engine.ISessionImplementor session, System.Collections.Generic.IDictionary<string, object> data, object newObj, object oldObj)
		{
		}

		public override void MapModifiedFlagsToMapForCollectionChange(string collectionPropertyName, System.Collections.Generic.IDictionary<string, object> data)
		{
			if (PropertyData.UsingModifiedFlag)
			{
				data[PropertyData.ModifiedFlagPropertyName] = collectionPropertyName.Equals(PropertyData.Name);
			}
		}
	}
}