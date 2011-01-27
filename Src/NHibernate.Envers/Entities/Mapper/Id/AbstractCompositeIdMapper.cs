using System;
using System.Collections.Generic;
using NHibernate.Envers.Exceptions;


namespace NHibernate.Envers.Entities.Mapper.Id
{
	public abstract class  AbstractCompositeIdMapper : AbstractIdMapper , ISimpleIdMapperBuilder
	{
		protected IDictionary<PropertyData, SingleIdMapper> ids;
		protected System.Type compositeIdClass;

		protected AbstractCompositeIdMapper(System.Type compositeIdClass)
		{
			ids = new Dictionary<PropertyData, SingleIdMapper>();

			this.compositeIdClass = compositeIdClass;
		}

		public void Add(PropertyData propertyData)
		{
			ids.Add(propertyData, new SingleIdMapper(propertyData));     
		}

		public override object MapToIdFromMap(IDictionary<string, object> data)
		{
			object ret;
			try 
			{
				ret = Activator.CreateInstance(compositeIdClass);
			} 
			catch (Exception e) 
			{
				throw new AuditException(e);
			}	

			foreach (var mapper in ids.Values) 
			{
				mapper.MapToEntityFromMap(ret, data);
			}

			return ret;
		}
	}
}
