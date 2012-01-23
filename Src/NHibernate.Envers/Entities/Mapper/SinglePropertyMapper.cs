using System.Collections;
using System.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;
using NHibernate.Envers.Tools.Reflection;

namespace NHibernate.Envers.Entities.Mapper
{
	public class SinglePropertyMapper : IPropertyMapper, ISimpleMapperBuilder 
	{
		private PropertyData _propertyData;

		public SinglePropertyMapper(PropertyData propertyData) 
		{
			_propertyData = propertyData;
		}

		public SinglePropertyMapper() { }

		public void Add(PropertyData propertyData) 
		{
			if (_propertyData != null) 
			{
				throw new AuditException("Only one property can be added!");
			}

			_propertyData = propertyData;
		}

		public bool MapToMapFromEntity(ISessionImplementor session, IDictionary<string, object> data, object newObj, object oldObj) 
		{
			data[_propertyData.Name] = newObj;
			if (newObj == null)
			{
				return oldObj != null;
			}
			return !newObj.Equals(oldObj);
		}

		public void MapToEntityFromMap(AuditConfiguration verCfg, object obj, IDictionary data, object primaryKey,
									   IAuditReaderImplementor versionsReader, long revision) 
		{
			if (data == null || obj == null) 
			{
				return;
			}
			var objType = obj.GetType();
			var setter = ReflectionTools.GetSetter(objType, _propertyData);

			var value = data[_propertyData.Name];
			setter.Set(obj, value);
		}

		public IList<PersistentCollectionChangeData> MapCollectionChanges(string referencingPropertyName,
																		 IPersistentCollection newColl,
																		 object oldColl,
																		 object id) 
		{
			return null;
		}

		public void MapModifiedFlagsToMapFromEntity(ISessionImplementor session, IDictionary<string, object> data, object newObj, object oldObj)
		{
			if (_propertyData.UsingModifiedFlag)
			{
				data[_propertyData.ModifiedFlagPropertyName] = !Toolz.ObjectsEqual(newObj, oldObj);
			}
		}

		public void MapModifiedFlagsToMapForCollectionChange(string collectionPropertyName, IDictionary<string, object> data)
		{
		}
	}
}
