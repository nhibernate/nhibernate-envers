﻿using NHibernate.Envers.Entities;
using NHibernate.Properties;

namespace NHibernate.Envers.Tools.Reflection
{
    public static class ReflectionTools
    {
        public static IGetter GetGetter(System.Type cls, PropertyData propertyData)
        {
            return GetGetter(cls, propertyData.BeanName, propertyData.AccessType);
        }

		public static IGetter GetGetter(System.Type cls, string propertyName, string accessorType)
        {
			var value = accessor(accessorType).GetGetter(cls, propertyName);

            return value;
        }

		private static IPropertyAccessor accessor(string accessorType)
    	{
    		return PropertyAccessorFactory.GetPropertyAccessor(accessorType);
    	}

		public static ISetter GetSetter(System.Type cls, PropertyData propertyData)
		{
			return GetSetter(cls, propertyData.BeanName, propertyData.AccessType);
		}

		private static ISetter GetSetter(System.Type cls, string propertyName, string accessorType)
		{	
			return accessor(accessorType).GetSetter(cls, propertyName);
		}
    }
}
