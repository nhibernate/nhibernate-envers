using System;
using System.Collections.Generic;

namespace NHibernate.Envers.Configuration
{
	public abstract class ConfigurationEntry<T>
	{
		private readonly string _defaultValueAsString;
		private readonly string _key;

		protected ConfigurationEntry(string key, string defaultValueAsString)
		{
			_key = key;
			_defaultValueAsString = defaultValueAsString;
		}

		protected string PropertyValue(IDictionary<string, string> nhibernateProperties)
		{
			string stringValue;
			if (!nhibernateProperties.TryGetValue(_key, out stringValue))
			{
				stringValue = _defaultValueAsString;
			}
			return stringValue;
		}

		public void SetUserValue(Cfg.Configuration nhibernateProperties, T value)
		{
			nhibernateProperties.SetProperty(_key, ToString(value));
		}

		protected abstract string ToString(T value);
	}

	public class StringConfigurationEntry : ConfigurationEntry<string>
	{
		public StringConfigurationEntry(string key, string defaultValueAsString) 
			: base(key, defaultValueAsString)
		{
		}

		public string ToString(IDictionary<string, string> nhibernateProperties)
		{
			return PropertyValue(nhibernateProperties);
		}

		protected override string ToString(string value)
		{
			return value;
		}
	}

	public class BoolConfigurationEntry : ConfigurationEntry<bool>
	{
		public BoolConfigurationEntry(string key, string defaultValueAsString) : base(key, defaultValueAsString)
		{
		}

		public bool ToBool(IDictionary<string, string> nhibernateProperties)
		{
			var propAsString = PropertyValue(nhibernateProperties);
			return "true".Equals(propAsString, StringComparison.InvariantCultureIgnoreCase);
		}

		protected override string ToString(bool value)
		{
			return value ? "true" : "false";
		}
	}

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