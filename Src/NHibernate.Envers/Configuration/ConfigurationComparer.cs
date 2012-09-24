using System.Collections.Generic;

namespace NHibernate.Envers.Configuration
{
	public class ConfigurationComparer : IEqualityComparer<Cfg.Configuration>
	{
		public bool Equals(Cfg.Configuration x, Cfg.Configuration y)
		{
			return ConfigurationKey.UniqueConfigurationName.ToString(x.Properties)
				.Equals(ConfigurationKey.UniqueConfigurationName.ToString(y.Properties));
		}

		public int GetHashCode(Cfg.Configuration obj)
		{
			return ConfigurationKey.RevisionFieldName.ToString(obj.Properties).GetHashCode();
		}
	}
}