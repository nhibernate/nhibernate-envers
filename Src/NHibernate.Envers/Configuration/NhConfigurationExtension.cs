using NHibernate.Envers.Configuration;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Event;
using NHibernate.Event;

namespace NHibernate.Cfg
{
	public static class NhConfigurationExtension
	{
		public static Configuration IntegrateWithEnvers(this Configuration configuration, 
														AuditEventListener auditEventListener,
														IMetaDataProvider metaDataProvider)
		{
			AuditConfiguration.SetConfigMetas(configuration, metaDataProvider);
			addListeners(configuration, auditEventListener);
			return configuration;
		}

		private static void addListeners(Configuration cfg, AuditEventListener auditEventListener)
		{
			var listeners = new[] { auditEventListener };
			cfg.AppendListeners(ListenerType.PostInsert, listeners);
			cfg.AppendListeners(ListenerType.PostUpdate, listeners);
			cfg.AppendListeners(ListenerType.PostDelete, listeners);
			cfg.AppendListeners(ListenerType.PostCollectionRecreate, listeners);
			cfg.AppendListeners(ListenerType.PreCollectionRemove, listeners);
			cfg.AppendListeners(ListenerType.PreCollectionUpdate, listeners);
		}
	}
}