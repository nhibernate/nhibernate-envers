using System;
using System.Collections.Generic;

namespace NHibernate.Envers.Configuration
{
	public class TypeConfigurationEntry : ConfigurationEntry<System.Type>
	{
		public TypeConfigurationEntry(string key, string defaultValueAsString) : base(key, defaultValueAsString)
		{
		}

		public T ToInstance<T>(IDictionary<string, string> nhibernateProperties)
		{
			var propAsType = ToType(nhibernateProperties);
			return (T)Activator.CreateInstance(propAsType);
		}

		public System.Type ToType(IDictionary<string, string> nhibernateProperties)
		{
			var propAsString = PropertyValue(nhibernateProperties);
			return System.Type.GetType(propAsString, true, true);
		}

		protected override string ToString(System.Type value)
		{
			return value.AssemblyQualifiedName;
		}
	}
}