using System.Collections.Generic;

namespace NHibernate.Envers.Configuration
{
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
}