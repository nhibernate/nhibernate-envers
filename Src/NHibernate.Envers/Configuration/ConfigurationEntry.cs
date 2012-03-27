using System;
using System.Collections.Generic;

namespace NHibernate.Envers.Configuration
{
	public abstract class ConfigurationEntry<T>
	{
		private readonly string _defaultValueAsString;

		protected ConfigurationEntry(string key, string defaultValueAsString)
		{
			Key = key;
			_defaultValueAsString = defaultValueAsString;
		}

		public string Key { get; private set; }

		public T ToValue(IDictionary<string, string> nhibernateProperties)
		{
			string stringValue;
			if (!nhibernateProperties.TryGetValue(Key, out stringValue))
			{
				stringValue = _defaultValueAsString;
			}
			return ToValue(stringValue);
		}

		public abstract string ToString(T value);
		protected abstract T ToValue(string value);
	}

	public class StringConfigurationEntry : ConfigurationEntry<string>
	{
		public StringConfigurationEntry(string key, string defaultValueAsString) 
			: base(key, defaultValueAsString)
		{
		}

		public override string ToString(string value)
		{
			return value;
		}

		protected override string ToValue(string value)
		{
			return value;
		}
	}

	public class BoolConfigurationEntry : ConfigurationEntry<bool>
	{
		public BoolConfigurationEntry(string key, string defaultValueAsString) : base(key, defaultValueAsString)
		{
		}

		public override string ToString(bool value)
		{
			return value ? "true" : "false";
		}

		protected override bool ToValue(string value)
		{
			return "true".Equals(value, StringComparison.OrdinalIgnoreCase);
		}
	}

	public class TypeConfigurationEntry : ConfigurationEntry<System.Type>
	{
		public TypeConfigurationEntry(string key, string defaultValueAsString) : base(key, defaultValueAsString)
		{
		}

		public override string ToString(System.Type value)
		{
			return value.AssemblyQualifiedName;
		}

		protected override System.Type ToValue(string value)
		{
			return System.Type.GetType(value, true, true);
		}
	}
}