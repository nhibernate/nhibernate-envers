using System;
using NHibernate.Envers.Query;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	[Serializable]
	public partial class OneToOneNotOwningMapper : AbstractOneToOneMapper
	{
		private readonly string _owningReferencePropertyName;

		public OneToOneNotOwningMapper(string notOwningEntityName, string owningEntityName, string owningReferencePropertyName, PropertyData propertyData)
			: base(notOwningEntityName, owningEntityName, propertyData)
		{
			_owningReferencePropertyName = owningReferencePropertyName;
		}

		protected override object QueryForReferencedEntity(IAuditReaderImplementor versionsReader, EntityInfo referencedEntity, object primaryKey, long revision, bool removed)
		{
			return versionsReader.CreateQuery().ForEntitiesAtRevision(referencedEntity.EntityClass, revision)
				.Add(AuditEntity.RelatedId(_owningReferencePropertyName).Eq(primaryKey))
				.GetSingleResult();
		}
	}
}
