using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Properties;
using NHibernate.Envers.Exceptions;
using NHibernate.Engine;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Configuration;
using NHibernate.Collection;

namespace NHibernate.Envers.Entities.Mapper
{
	public class SinglePropertyMapper : IPropertyMapper, ISimpleMapperBuilder 
	{
		private PropertyData propertyData;

		//rk - why not IsPrimitive instead?
		private static readonly List<System.Type> primitiveTypesList = new List<System.Type>{ typeof(bool), typeof(byte), 
						typeof(char), typeof(short), typeof(int), typeof(long), typeof(float), typeof(double)};

		public SinglePropertyMapper(PropertyData propertyData) 
		{
			this.propertyData = propertyData;
		}

		public SinglePropertyMapper() { }

		public void Add(PropertyData propertyData) 
		{
			if (this.propertyData != null) 
			{
				throw new AuditException("Only one property can be added!");
			}

			this.propertyData = propertyData;
		}

		public bool MapToMapFromEntity(ISessionImplementor session, IDictionary<string, object> data, object newObj, object oldObj) 
		{
			data[propertyData.Name] = newObj;
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
			var setter = ReflectionTools.GetSetter(objType, propertyData);

			var value = data[propertyData.Name];
			// We only set a null value if the field is not primitive. Otherwise, we leave it intact.
			if (value != null || !isPrimitive(setter, objType)) 
			{
				setter.Set(obj, value);
			}
		}

		private bool isPrimitive(ISetter setter, System.Type cls) 
		{
			if (cls == null) 
			{
				throw new HibernateException("No field found for property: " + setter.PropertyName);
			}
			//rk: I'm not sure about this one...
			//this won't work if mapped "readonly"
			if (setter.Method == null)
			{
				var field = cls.GetField(propertyData.BeanName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
				if (field != null)
					return field.GetType().IsPrimitive;
				return isPrimitive(setter, cls.BaseType);
			}
			return primitiveTypesList.Contains(setter.Method.GetParameters()[0].ParameterType);
		}

		public IList<PersistentCollectionChangeData> MapCollectionChanges(string referencingPropertyName,
																		 IPersistentCollection newColl,
																		 object oldColl,
																		 object id) {
			return null;
		}
	}
}
