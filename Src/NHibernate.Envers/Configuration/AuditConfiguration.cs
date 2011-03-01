using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Metadata.Reader;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Entities;
using NHibernate.Envers.RevisionInfo;
using NHibernate.Envers.Synchronization;

namespace NHibernate.Envers.Configuration
{
	public class AuditConfiguration
	{
		private static readonly IDictionary<Cfg.Configuration, AuditConfiguration> Configurations = new Dictionary<Cfg.Configuration, AuditConfiguration>();
		private static readonly IDictionary<Cfg.Configuration, IMetaDataProvider> ConfigurationMetadataProvider = new Dictionary<Cfg.Configuration, IMetaDataProvider>();

		private AuditConfiguration(Cfg.Configuration cfg, IMetaDataProvider metaDataProvider)
		{
			var mds = new MetaDataStore(cfg, metaDataProvider);

			var properties = cfg.Properties;
			var propertyAndMemberInfo = new PropertyAndMemberInfo();
			var revInfoCfg = new RevisionInfoConfiguration(mds, propertyAndMemberInfo);
			var revInfoCfgResult = revInfoCfg.Configure(cfg);
			AuditEntCfg = new AuditEntitiesConfiguration(properties, revInfoCfgResult.RevisionInfoEntityName);
			GlobalCfg = new GlobalConfiguration(properties);
			AuditSyncManager = new AuditSyncManager(revInfoCfgResult.RevisionInfoGenerator);
			RevisionInfoQueryCreator = revInfoCfgResult.RevisionInfoQueryCreator;
			RevisionInfoNumberReader = revInfoCfgResult.RevisionInfoNumberReader;
			EntCfg = new EntitiesConfigurator().Configure(cfg, mds, GlobalCfg, AuditEntCfg,
			                                              revInfoCfgResult.RevisionInfoXmlMapping, revInfoCfgResult.RevisionInfoRelationMapping, propertyAndMemberInfo);
		}

		public GlobalConfiguration GlobalCfg { get; private set; }
		public AuditEntitiesConfiguration AuditEntCfg { get; private set; }
		public AuditSyncManager AuditSyncManager { get; private set; }
		public EntitiesConfigurations EntCfg { get; private set; }
		public RevisionInfoQueryCreator RevisionInfoQueryCreator { get; private set; }
		public RevisionInfoNumberReader RevisionInfoNumberReader { get; private set; }


		[MethodImpl(MethodImplOptions.Synchronized)]
		public static AuditConfiguration GetFor(Cfg.Configuration cfg)
		{
			AuditConfiguration verCfg;
			if (!Configurations.TryGetValue(cfg, out verCfg))
			{
				cfg.BuildMappings(); // force secondpass for mappings added by users
				IMetaDataProvider metas;
				if (!ConfigurationMetadataProvider.TryGetValue(cfg, out metas))
				{
					metas = new AttributeConfiguration();
				}
				verCfg = new AuditConfiguration(cfg, metas);
				Configurations.Add(cfg, verCfg);
			}

			return verCfg;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void SetConfigMetas(Cfg.Configuration cfg, IMetaDataProvider metaDataProvider)
		{
			ConfigurationMetadataProvider[cfg] = metaDataProvider;
		}
	}
}