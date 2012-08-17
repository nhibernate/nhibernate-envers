using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Tools;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Mapping;

namespace NHibernate.Envers.Configuration.Store
{
	/// <summary>
	/// Adds <see cref="AuditMappedByAttribute"/> to biref collection with
	/// one side set to update=false, insert=false.
	/// </summary>
	public class AuditMappedByMetaDataAdder : IMetaDataAdder
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(AuditMappedByMetaDataAdder));
		private readonly Cfg.Configuration _nhibernateConfiguration;

		public AuditMappedByMetaDataAdder(Cfg.Configuration nhibernateConfiguration)
		{
			_nhibernateConfiguration = nhibernateConfiguration;
		}

		public void AddMetaDataTo(IDictionary<System.Type, IEntityMeta> currentMetaData)
		{
			addBidirectionalInfo(currentMetaData);
		}

		private void addBidirectionalInfo(IDictionary<System.Type, IEntityMeta> metas)
		{
			foreach (var type in metas.Keys)
			{
				var persistentClass = _nhibernateConfiguration.GetClassMapping(type);
				if (persistentClass == null) continue;
				foreach (var property in persistentClass.PropertyIterator)
				{
					//is it a collection?
					var collectionValue = property.Value as Mapping.Collection;
					if (collectionValue == null) continue;

					//find referenced entity name
					var referencedEntity = MappingTools.ReferencedEntityName(property.Value);
					if (referencedEntity == null) continue;


					var refPersistentClass = _nhibernateConfiguration.GetClassMapping(referencedEntity);
					foreach (var refProperty in refPersistentClass.PropertyClosureIterator)
					{
						var attr = createAuditMappedByAttributeIfReferenceImmutable(collectionValue, refProperty);
						if (attr == null) continue;
						mightAddIndexToAttribute(attr, collectionValue, refPersistentClass.PropertyClosureIterator);
						var entityMeta = (EntityMeta)metas[type];
						var methodInfo = PropertyAndMemberInfo.PersistentInfo(type, new[] { property }).First().Member;
						addToEntityMeta(attr, entityMeta, methodInfo);
					}
				}
			}
		}

		private static void addToEntityMeta(AuditMappedByAttribute attribute, EntityMeta entityMeta, MemberInfo memberInfo)
		{
			log.DebugFormat("Adding AuditMappedByAttribute [MappedBy={0}, PositionMappedBy={1}] to member {2}",
												 attribute.MappedBy, attribute.PositionMappedBy, memberInfo);
			entityMeta.AddMemberMeta(memberInfo, attribute);
		}

		private static AuditMappedByAttribute createAuditMappedByAttributeIfReferenceImmutable(Mapping.Collection collectionValue, Property referencedProperty)
		{
			AuditMappedByAttribute attr = null;
		
			//check if bidrectional
			//TODO: backref check here is wrong! But if it's fixed it will be a big breaking change I guess.. For now, an extra table will be created.
			if (!referencedProperty.BackRef && MappingTools.SameColumns(referencedProperty.ColumnIterator, collectionValue.Key.ColumnIterator))
			{
				attr = new AuditMappedByAttribute {MappedBy = referencedProperty.Name};
				if (!referencedProperty.IsUpdateable && 
					!referencedProperty.IsInsertable &&
					//check that non update/insert properties are not also part of id!
					!MappingTools.AnyColumnMatches(referencedProperty.ColumnIterator, referencedProperty.PersistentClass.Identifier.ColumnIterator))
				{
					attr.ForceInsertOverride = true;									
				}
			}
			return attr;
		}

		private static void mightAddIndexToAttribute(AuditMappedByAttribute auditMappedByAttribute, Mapping.Collection collectionValue, IEnumerable<Property> referencedProperties)
		{
			//check index value
			var indexValue = collectionValue as IndexedCollection;
			if (indexValue == null) return;
			foreach (var referencedProperty in referencedProperties)
			{
				if (MappingTools.SameColumns(referencedProperty.ColumnIterator, indexValue.Index.ColumnIterator) &&
												   !referencedProperty.IsUpdateable &&
												   !referencedProperty.IsInsertable)
				{
					auditMappedByAttribute.PositionMappedBy = referencedProperty.Name;
				}
			}
		}

	}
}