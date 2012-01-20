using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Id;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	public class ToOneIdMapper : AbstractToOneMapper 
	{
		private readonly IIdMapper _delegat;
		private readonly PropertyData _propertyData;
		private readonly string _referencedEntityName;
		private readonly bool _nonInsertableFake;

		public ToOneIdMapper(IIdMapper delegat, PropertyData propertyData, string referencedEntityName, bool nonInsertableFake)
			: base(propertyData)
		{
			_delegat = delegat;
			_propertyData = propertyData;
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

			//noinspection SimplifiableConditionalExpression
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
					value = versionsReader.SessionImplementor.Factory.GetEntityPersister(_referencedEntityName)
						.CreateProxy(entityId,
						             new ToOneDelegateSessionImplementor(versionsReader, entityId, revision, verCfg));
				}
			}
			SetPropertyValue(obj, value);
		}
	}
}
