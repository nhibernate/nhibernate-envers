using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Id;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	[Serializable]
	public class ToOneIdMapper : AbstractToOneMapper 
	{
		private readonly IEnversProxyFactory _enversProxyFactory;
		private readonly IIdMapper _delegat;
		private readonly string _referencedEntityName;
		private readonly bool _nonInsertableFake;

		public ToOneIdMapper(IEnversProxyFactory enversProxyFactory, 
									IIdMapper delegat, 
									PropertyData propertyData, 
									string referencedEntityName, 
									bool nonInsertableFake)
			: base(propertyData)
		{
			_enversProxyFactory = enversProxyFactory;
			_delegat = delegat;
			_referencedEntityName = referencedEntityName;
			_nonInsertableFake = nonInsertableFake;
		}

		public override bool MapToMapFromEntity(ISessionImplementor session, IDictionary<string, object> data, object newObj, object oldObj)
		{
			var newData = new Dictionary<string, object>();

			// If this property is originally non-insertable, but made insertable because it is in a many-to-one "fake"
			// bi-directional relation, we always store the "old", unchaged data, to prevent storing changes made
			// to this field. It is the responsibility of the collection to properly update it if it really changed.
			_delegat.MapToMapFromEntity(newData, _nonInsertableFake ? oldObj : newObj);

			foreach (var entry in newData)
			{
				data[entry.Key] = entry.Value;
			}

			return CheckModified(session, newObj, oldObj);
		}

		public override void MapModifiedFlagsToMapFromEntity(ISessionImplementor session, IDictionary<string, object> data, object newObj, object oldObj)
		{
			if (PropertyData.UsingModifiedFlag)
			{
				data[PropertyData.ModifiedFlagPropertyName] = CheckModified(session, newObj, oldObj);
			}
		}

		public override void MapModifiedFlagsToMapForCollectionChange(string collectionPropertyName, IDictionary<string, object> data)
		{
			if (PropertyData.UsingModifiedFlag)
			{
				data[PropertyData.ModifiedFlagPropertyName] = collectionPropertyName.Equals(PropertyData.Name);
			}
		}

		protected bool CheckModified(ISessionImplementor session, object newObj, object oldObj)
		{
			return !_nonInsertableFake && !Toolz.EntitiesEqual(session, newObj, oldObj);
		}

		protected override void NullSafeMapToEntityFromMap(AuditConfiguration verCfg, object obj, IDictionary data, object primaryKey, IAuditReaderImplementor versionsReader, long revision)
		{
			var entityId = _delegat.MapToIdFromMap(data);
			object value = null;
			if (entityId != null)
			{
				if (!versionsReader.FirstLevelCache.TryGetValue(_referencedEntityName, revision, entityId, out value))
				{
					value = _enversProxyFactory.CreateToOneProxy(verCfg, versionsReader, _referencedEntityName, entityId, revision);
				}
			}
			SetPropertyValue(obj, value);
		}
	}
}
