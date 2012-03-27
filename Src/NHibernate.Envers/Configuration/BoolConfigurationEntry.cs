using System;
using System.Collections.Generic;

namespace NHibernate.Envers.Configuration
{
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
}