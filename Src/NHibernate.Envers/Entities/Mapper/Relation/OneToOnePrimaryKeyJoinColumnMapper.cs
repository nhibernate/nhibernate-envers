using System;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	[Serializable]
	public partial class OneToOnePrimaryKeyJoinColumnMapper : AbstractOneToOneMapper
	{
		public OneToOnePrimaryKeyJoinColumnMapper(string entityName, string referencedEntityName, PropertyData propertyData)
			: base(entityName, referencedEntityName, propertyData)
		{
		}

		protected override object QueryForReferencedEntity(IAuditReaderImplementor versionsReader, EntityInfo referencedEntity, object primaryKey, long revision, bool removed)
		{
			if (referencedEntity.IsAudited)
			{
				return versionsReader.Find(referencedEntity.EntityName, primaryKey, revision, removed);
			}
			//Not audited revision
			return createNotAuditedEntityReference(versionsReader, referencedEntity.EntityClass,
			                                       referencedEntity.EntityName, primaryKey);
		}

		private static object createNotAuditedEntityReference(IAuditReaderImplementor versionsReader, System.Type entityClass, string entityName, object primaryKey)
		{
			var entityPersister = versionsReader.SessionImplementor.Factory.GetEntityPersister(entityName);
			if (entityPersister.HasProxy)
			{
				// If possible create a proxy. Returning complete object may affect performance.
				return versionsReader.Session.Load(entityClass, primaryKey);
			}
			// If proxy is not allowed (e.g. @Proxy(lazy=false)) construct the original object.
			return versionsReader.Session.Get(entityClass, primaryKey);
		}
	}
}