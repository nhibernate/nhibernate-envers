using System.Linq;

namespace NHibernate.Envers.Tests.Tools
{
	public static class ConfigurationExtensions
	{
		public static Cfg.Configuration OverrideSettingsFromEnvironmentVariables(this Cfg.Configuration configuration)
		{
			foreach (var nhProperty in configuration.Properties.ToArray())
			{
				var envVar = System.Environment.GetEnvironmentVariable(nhProperty.Key);
				if (envVar != null)
				{
					configuration.SetProperty(nhProperty.Key, envVar);
				}
			}
			
			return configuration;
		}
	}
}