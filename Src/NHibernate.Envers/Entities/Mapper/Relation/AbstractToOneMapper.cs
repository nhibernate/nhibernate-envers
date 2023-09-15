using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;
using NHibernate.Envers.Tools.Reflection;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	[Serializable]
	public abstract partial class AbstractToOneMapper : IPropertyMapper
	{
		protected AbstractToOneMapper(PropertyData propertyData)
		{
			PropertyData = propertyData;
		}

		protected PropertyData PropertyData { get; }

		public virtual bool MapToMapFromEntity(ISessionImplementor session, IDictionary<string, object> data, object newObj, object oldObj)
		{
			return false;
		}

		public void MapToEntityFromMap(AuditConfiguration verCfg, object obj, IDictionary data, object primaryKey, IAuditReaderImplementor versionsReader, long revision)
		{
			if (obj != null)
			{
				NullSafeMapToEntityFromMap(verCfg, obj, data, primaryKey, versionsReader, revision);
			}
		}

		public IList<PersistentCollectionChangeData> MapCollectionChanges(ISessionImplementor session, string referencingPropertyName, IPersistentCollection newColl, object oldColl, object id)
		{
			return null;
		}

		public abstract void MapModifiedFlagsToMapFromEntity(ISessionImplementor session, IDictionary<string, object> data, object newObj, object oldObj);
		public abstract void MapModifiedFlagsToMapForCollectionChange(string collectionPropertyName, IDictionary<string, object> data);

		protected EntityInfo GetEntityInfo(AuditConfiguration verCfg, string entityName)
		{
			var entCfg = verCfg.EntCfg[entityName];
			var isRelationAudited = true;
			if (entCfg == null)
			{
				// a relation marked as RelationTargetAuditMode.NOT_AUDITED
				entCfg = verCfg.EntCfg.GetNotVersionEntityConfiguration(entityName);
				isRelationAudited = false;
			}
			var entityClass = Toolz.ResolveDotnetType(entCfg.EntityClassName);
			return new EntityInfo(entityClass, entityName, isRelationAudited);
		}

		protected void SetPropertyValue(object targetObject, object value)
		{
			var setter = ReflectionTools.GetSetter(targetObject.GetType(), PropertyData);
			setter.Set(targetObject, value);
		}


		protected abstract void NullSafeMapToEntityFromMap(AuditConfiguration verCfg, object obj, IDictionary data,
		                                                   object primaryKey, IAuditReaderImplementor versionsReader, long revision);

		protected class EntityInfo
		{
			public EntityInfo(System.Type entityClass, string entityName, bool isAudited)
			{
				EntityClass = entityClass;
				EntityName = entityName;
				IsAudited = isAudited;
			}

			public System.Type EntityClass { get; }
			public string EntityName { get; }
			public bool IsAudited { get; }

		}
	}
}