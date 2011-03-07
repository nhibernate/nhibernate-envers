﻿using System.Collections;
using System.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Query;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;
using NHibernate.Envers.Tools.Reflection;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	public class OneToOneNotOwningMapper : IPropertyMapper 
	{
		private readonly string owningReferencePropertyName;
		private readonly string owningEntityName;
		private readonly PropertyData propertyData;

		public OneToOneNotOwningMapper(string owningReferencePropertyName, 
										string owningEntityName,
										PropertyData propertyData) 
		{
			this.owningReferencePropertyName = owningReferencePropertyName;
			this.owningEntityName = owningEntityName;
			this.propertyData = propertyData;
		}

		public bool MapToMapFromEntity(ISessionImplementor session, IDictionary<string, object> data, object newObj, object oldObj) 
		{
			return false;
		}

		public void MapToEntityFromMap(AuditConfiguration verCfg, object obj, IDictionary data, object primaryKey, IAuditReaderImplementor versionsReader, long revision) 
		{
			if (obj == null) 
			{
				return;
			}

			var entityClass = Toolz.ResolveDotnetType(owningEntityName);

			object value;

			try
			{
				value = versionsReader.CreateQuery().ForEntitiesAtRevision(entityClass, revision)
						.Add(AuditEntity.RelatedId(owningReferencePropertyName).Eq(primaryKey)).GetSingleResult();
			}
			catch (NoResultException)
			{
				value = null;
			}
			catch (NonUniqueResultException)
			{
				throw new AuditException("Many versions results for one-to-one relationship: (" + owningEntityName +
						", " + owningReferencePropertyName + ")");
			}

			var setter = ReflectionTools.GetSetter(obj.GetType(), propertyData);
			setter.Set(obj, value);
		}

		public IList<PersistentCollectionChangeData> MapCollectionChanges(string referencingPropertyName,
																						IPersistentCollection newColl,
																						object oldColl,
																						object id)
		{
			return null;
		}
	}
}
