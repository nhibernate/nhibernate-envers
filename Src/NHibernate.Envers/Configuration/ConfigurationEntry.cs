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
}