using NHibernate.Envers.Configuration;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Event;
using NHibernate.Event;

namespace NHibernate.Cfg
{
	public static class NhConfigurationExtension
	{
		/// <summary>
		/// Integrate Envers with NHibernate.
		/// </summary>
		/// <param name="configuration">The NHibernate configuration.</param>
		/// <param name="auditEventListener">The custom implementation for all listeners (better if inherited from <see cref="AuditEventListener"/>>)</param>
		/// <param name="metaDataProvider">The provider of metadata (attributes, embedded fluent-configuration or custom <see cref="IMetaDataProvider"/> for custom DSL.</param>
		/// <returns>The NHibernate configuration.</returns>
		/// <remarks>
		/// WARNING: Be sure to call this method after set all configuration properties, after have added all your mappings 
		/// and after integrate NHibernate with all others packages as NHibernate.Validator, NHibernate.Search, NHibernate.Spatial.
		/// </remarks>
		public static Configuration IntegrateWithEnvers(this Configuration configuration, AuditEventListener auditEventListener, IMetaDataProvider metaDataProvider)
		{
			AuditConfiguration.SetConfigMetas(configuration, metaDataProvider);
			AddListeners(configuration, auditEventListener);
			AuditConfiguration.GetFor(configuration);
			return configuration;
		}

		/// <summary>
		/// Integrate Envers with NHibernate.
		/// </summary>
		/// <param name="configuration">The NHibernate configuration.</param>
		/// <param name="metaDataProvider">The provider of metadata (attributes, embedded fluent-configuration or custom <see cref="IMetaDataProvider"/> for custom DSL.</param>
		/// <returns>The NHibernate configuration.</returns>
		/// <remarks>
		/// The default <see cref="AuditEventListener"/> will be used.
		/// WARNING: Be sure to call this method after set all configuration properties, after have added all your mappings 
		/// and after integrate NHibernate with all others packages as NHibernate.Validator, NHibernate.Search, NHibernate.Spatial.
		/// </remarks>
		/// <seealso cref="AttributeConfiguration"/>
		/// <seealso cref="NHibernate.Envers.Configuration.Fluent.FluentConfiguration"/>
		public static Configuration IntegrateWithEnvers(this Configuration configuration, IMetaDataProvider metaDataProvider)
		{
			return IntegrateWithEnvers(configuration, new AuditEventListener(), metaDataProvider);
		}

		/// <summary>
		/// Integrate Envers with NHibernate.
		/// </summary>
		/// <param name="configuration">The NHibernate configuration.</param>
		/// <returns>The NHibernate configuration.</returns>
		/// <remarks>
		/// The default <see cref="AuditEventListener"/> and the <see cref="AttributeConfiguration"/> will be used.
		/// WARNING: Be sure to call this method after set all configuration properties, after have added all your mappings 
		/// and after integrate NHibernate with all others packages as NHibernate.Validator, NHibernate.Search, NHibernate.Spatial.
		/// </remarks>
		public static Configuration IntegrateWithEnvers(this Configuration configuration)
		{
			return IntegrateWithEnvers(configuration, new AuditEventListener(), new AttributeConfiguration());
		}

		private static void AddListeners(Configuration cfg, AuditEventListener auditEventListener)
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