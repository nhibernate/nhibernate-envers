using System;

namespace NHibernate.Envers.Configuration
{
	public class ConfigurationEntry<T>
	{
		public ConfigurationEntry(string key, string defaultValueAsString, Func<T, string> toStringFunc)
		{
			Key = key;
			DefaultValueAsString = defaultValueAsString;
			ToStringFunc = toStringFunc;
		}

		public string Key { get; private set; }
		public string DefaultValueAsString { get; private set; }
		public Func<T, string> ToStringFunc { get; private set; }
	}
}