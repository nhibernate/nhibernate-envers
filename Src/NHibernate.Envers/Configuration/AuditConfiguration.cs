using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Entities;
using NHibernate.Envers.RevisionInfo;
using NHibernate.Envers.Synchronization;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Properties;

namespace NHibernate.Envers.Configuration
{
	[Serializable]
	public sealed class AuditConfiguration : IDeserializationCallback
	{
		private static readonly IDictionary<Cfg.Configuration, AuditConfiguration> configurations = new Dictionary<Cfg.Configuration, AuditConfiguration>(new ConfigurationComparer());
		private static readonly IDictionary<Cfg.Configuration, IMetaDataProvider> configurationMetadataProvider = new Dictionary<Cfg.Configuration, IMetaDataProvider>();

		private AuditConfiguration(Cfg.Configuration cfg, IMetaDataProvider metaDataProvider)
		{
			//this might be over kill - move back into MetaDataStore later if not needed for other stuff...
			var metaDataAdders = new List<IMetaDataAdder> {new AuditMappedByMetaDataAdder(cfg)};

			var mds = new MetaDataStore(cfg, metaDataProvider, metaDataAdders);
			var properties = cfg.Properties;
			GlobalCfg = new GlobalConfiguration(this, properties);

			var revInfoCfg = new RevisionInfoConfiguration(GlobalCfg, mds);
			var revInfoCfgResult = revInfoCfg.Configure(cfg);
			AuditEntCfg = new AuditEntitiesConfiguration(properties, revInfoCfgResult.RevisionInfoEntityName);
			AuditProcessManager = new AuditProcessManager(revInfoCfgResult.RevisionInfoGenerator);

			RevisionTimestampGetter = ReflectionTools.GetGetter(revInfoCfgResult.RevisionInfoClass, revInfoCfgResult.RevisionInfoTimestampData);

			RevisionInfoQueryCreator = revInfoCfgResult.RevisionInfoQueryCreator;
			RevisionInfoNumberReader = revInfoCfgResult.RevisionInfoNumberReader;
			ModifiedEntityNamesReader = revInfoCfgResult.ModifiedEntityNamesReader;
			EntCfg = new EntitiesConfigurator()
				.Configure(cfg, mds, GlobalCfg, AuditEntCfg, revInfoCfgResult.RevisionInfoXmlMapping, revInfoCfgResult.RevisionInfoRelationMapping);
			Configuration = cfg;
		}

		public GlobalConfiguration GlobalCfg { get; private set; }
		public AuditEntitiesConfiguration AuditEntCfg { get; private set; }
		public AuditProcessManager AuditProcessManager { get; private set; }
		public EntitiesConfigurations EntCfg { get; private set; }
		public RevisionInfoQueryCreator RevisionInfoQueryCreator { get; private set; }
		public RevisionInfoNumberReader RevisionInfoNumberReader { get; private set; }
		public ModifiedEntityNamesReader ModifiedEntityNamesReader { get; private set; }
		public IGetter RevisionTimestampGetter { get; private set; }
		private Cfg.Configuration Configuration { get; set; } //for serialization


		[MethodImpl(MethodImplOptions.Synchronized)]
		public static AuditConfiguration GetFor(Cfg.Configuration cfg)
		{
			AuditConfiguration verCfg;
			if (!configurations.TryGetValue(cfg, out verCfg))
			{
				cfg.SetEnversProperty(ConfigurationKey.UniqueConfigurationName, Guid.NewGuid().ToString());
				cfg.BuildMappings(); // force secondpass for mappings added by users
				IMetaDataProvider metas;
				if (!configurationMetadataProvider.TryGetValue(cfg, out metas))
				{
					metas = new AttributeConfiguration();
				}
				verCfg = new AuditConfiguration(cfg, metas);
				configurations.Add(cfg, verCfg);
			}

			return verCfg;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void SetConfigMetas(Cfg.Configuration cfg, IMetaDataProvider metaDataProvider)
		{
			configurationMetadataProvider[cfg] = metaDataProvider;
		}

		/// <summary>
		/// Release Envers resources.
		/// </summary>
		/// <param name="cfg">
		/// The <see cref="Cfg.Configuration"/> object belonging to current <see cref="ISessionFactory"/>.
		/// </param>
		/// <returns>
		/// Normally not needed to be called.
		/// Should be used in environment where multiple Envers aware 
		/// <see cref="ISessionFactory"/> are created and destroyed during runtime.
		/// Call this method when <see cref="ISessionFactory"/> is disposed.
		/// </returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void Remove(Cfg.Configuration cfg)
		{
			configurationMetadataProvider.Remove(cfg);
			configurations.Remove(cfg);
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
			configurations[Configuration] = this;
		}
	}
}