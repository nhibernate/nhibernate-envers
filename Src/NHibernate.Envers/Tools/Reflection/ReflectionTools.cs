using System;
using NHibernate.Envers.Entities;
using NHibernate.Properties;
using NHibernate.Util;

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
			return accessor(accessorType).GetGetter(cls, propertyName);
		}

		private static IPropertyAccessor accessor(string accessorType)
		{
			return PropertyAccessorFactory.GetPropertyAccessor(accessorType);
		}

		public static ISetter GetSetter(System.Type cls, PropertyData propertyData)
		{
			return getSetter(cls, propertyData.BeanName, propertyData.AccessType);
		}

		private static ISetter getSetter(System.Type cls, string propertyName, string accessorType)
		{
			return accessor(accessorType).GetSetter(cls, propertyName);
		}

		public static object CreateInstanceByDefaultConstructor(System.Type type)
		{
			return type.IsValueType ? 
						Activator.CreateInstance(type) : 
						ReflectHelper.GetDefaultConstructor(type).Invoke(null);
		}
	}
}
