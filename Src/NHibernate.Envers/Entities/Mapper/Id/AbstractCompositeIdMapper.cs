using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Exceptions;

namespace NHibernate.Envers.Entities.Mapper.Id
{
	public abstract class AbstractCompositeIdMapper : AbstractIdMapper , ISimpleIdMapperBuilder
	{
		protected AbstractCompositeIdMapper(System.Type compositeIdClass)
		{
			Ids = new Dictionary<PropertyData, SingleIdMapper>();
			CompositeIdClass = compositeIdClass;
		}

		protected IDictionary<PropertyData, SingleIdMapper> Ids { get; private set; }
		protected System.Type CompositeIdClass { get; private set; }

		public void Add(PropertyData propertyData)
		{
			Ids.Add(propertyData, new SingleIdMapper(propertyData));     
		}

		public override object MapToIdFromMap(IDictionary data)
		{
			if (data == null)
				return null;
			object ret;
			try 
			{
				ret = Activator.CreateInstance(CompositeIdClass);
			} 
			catch (Exception e) 
			{
				throw new AuditException("Cannot create instance of type " + CompositeIdClass, e);
			}	

			foreach (var mapper in Ids.Values) 
			{
				mapper.MapToEntityFromMap(ret, data);
			}

			return ret;
		}
	}
}
