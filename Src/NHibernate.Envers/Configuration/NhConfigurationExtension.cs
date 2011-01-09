using NHibernate.Envers.Event;
using NHibernate.Event;

namespace NHibernate.Cfg
{
	public static class NhConfigurationExtension
	{
		public static Cfg.Configuration IntegrateWithEnvers(this Configuration configuration)
		{
			addListeners(configuration);
			return configuration;
		}

		private static void addListeners(Cfg.Configuration cfg)
		{
			var listeners = new[] { new AuditEventListener() };
			cfg.AppendListeners(ListenerType.PostInsert, listeners);
			cfg.AppendListeners(ListenerType.PostUpdate, listeners);
			cfg.AppendListeners(ListenerType.PostDelete, listeners);
			cfg.AppendListeners(ListenerType.PostCollectionRecreate, listeners);
			cfg.AppendListeners(ListenerType.PreCollectionRemove, listeners);
			cfg.AppendListeners(ListenerType.PreCollectionUpdate, listeners);
		}
	}
}