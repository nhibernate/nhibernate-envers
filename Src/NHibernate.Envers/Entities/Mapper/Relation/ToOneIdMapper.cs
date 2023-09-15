using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Id;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	[Serializable]
	public partial class ToOneIdMapper : AbstractToOneMapper 
	{
		private readonly IIdMapper _delegat;
		private readonly string _referencedEntityName;
		private readonly bool _nonInsertableFake;

		public ToOneIdMapper(IIdMapper delegat, 
							PropertyData propertyData, 
							string referencedEntityName, 
							bool nonInsertableFake)
			: base(propertyData)
		{
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

			return checkModified(session, newObj, oldObj);
		}

		public override void MapModifiedFlagsToMapFromEntity(ISessionImplementor session, IDictionary<string, object> data, object newObj, object oldObj)
		{
			if (PropertyData.UsingModifiedFlag)
			{
				data[PropertyData.ModifiedFlagPropertyName] = checkModified(session, newObj, oldObj);
			}
		}

		public override void MapModifiedFlagsToMapForCollectionChange(string collectionPropertyName, IDictionary<string, object> data)
		{
			if (PropertyData.UsingModifiedFlag)
			{
				data[PropertyData.ModifiedFlagPropertyName] = collectionPropertyName.Equals(PropertyData.Name);
			}
		}

		private bool checkModified(ISessionImplementor session, object newObj, object oldObj)
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
					var referencedEntity = GetEntityInfo(verCfg, _referencedEntityName);
					var ignoreNotFound = false;
					if (!referencedEntity.IsAudited)
					{
						var referencingEntityName = verCfg.EntCfg.GetEntityNameForVersionsEntityName((string) data["$type$"]);
						var relation = verCfg.EntCfg.GetRelationDescription(referencingEntityName, PropertyData.Name);
						ignoreNotFound = relation != null && relation.IsIgnoreNotFound;
					}
					var removed = RevisionType.Deleted.Equals(data[verCfg.AuditEntCfg.RevisionTypePropName]);

					value = ignoreNotFound ? 
						ToOneEntityLoader.LoadImmediate(versionsReader, _referencedEntityName, entityId, revision, removed, verCfg) : 
						ToOneEntityLoader.CreateProxyOrLoadImmediate(versionsReader, _referencedEntityName, entityId, revision, removed, verCfg);
				}
			}
			SetPropertyValue(obj, value);
		}

		public void AddMiddleEqualToQuery(Parameters parameters, string prefix1, string prefix2)
		{
			_delegat.AddIdsEqualToQuery(parameters, prefix1, _delegat, prefix2);
		}
	}
}
