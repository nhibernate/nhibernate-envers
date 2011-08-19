using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Entities;
using NHibernate.Envers.RevisionInfo;
using NHibernate.Envers.Strategy;
using NHibernate.Envers.Synchronization;
using NHibernate.Envers.Tools.Reflection;

namespace NHibernate.Envers.Configuration
{
	public class AuditConfiguration
	{
		private static readonly IDictionary<Cfg.Configuration, AuditConfiguration> Configurations = new Dictionary<Cfg.Configuration, AuditConfiguration>();
		private static readonly IDictionary<Cfg.Configuration, IMetaDataProvider> ConfigurationMetadataProvider = new Dictionary<Cfg.Configuration, IMetaDataProvider>();

		private AuditConfiguration(Cfg.Configuration cfg, IMetaDataProvider metaDataProvider)
		{
			//this might be over kill - move back into MetaDataStore later if not needed for other stuff...
			var metaDataAdders = new List<IMetaDataAdder> {new AuditMappedByMetaDataAdder(cfg)};

			var mds = new MetaDataStore(cfg, metaDataProvider, metaDataAdders);
			var properties = cfg.Properties;
			GlobalCfg = new GlobalConfiguration(properties);
			var revInfoCfg = new RevisionInfoConfiguration(GlobalCfg, mds);
			var revInfoCfgResult = revInfoCfg.Configure(cfg);
			AuditEntCfg = new AuditEntitiesConfiguration(properties, revInfoCfgResult.RevisionInfoEntityName);
			AuditProcessManager = new AuditProcessManager(revInfoCfgResult.RevisionInfoGenerator);
			initializeAuditStrategy(revInfoCfgResult);

			RevisionInfoQueryCreator = revInfoCfgResult.RevisionInfoQueryCreator;
			RevisionInfoNumberReader = revInfoCfgResult.RevisionInfoNumberReader;
			EntCfg = new EntitiesConfigurator().Configure(cfg, mds, GlobalCfg, AuditEntCfg, AuditStrategy,
			                                              revInfoCfgResult.RevisionInfoXmlMapping, revInfoCfgResult.RevisionInfoRelationMapping);
		}

		private void initializeAuditStrategy(RevisionInfoConfigurationResult revInfoCfgResult)
		{
			try
			{
				AuditStrategy = (IAuditStrategy) Activator.CreateInstance(AuditEntCfg.AuditStrategyType);
			}
			catch (Exception)
			{
				throw new MappingException(string.Format("Unable to create AuditStrategy[{0}] instance.", AuditEntCfg.AuditStrategyType.FullName));
			}
			var validityAuditStrategy = AuditStrategy as ValidityAuditStrategy;
			if(validityAuditStrategy!=null)
			{
				var revisionTimestampGetter = ReflectionTools.GetGetter(revInfoCfgResult.RevisionInfoClass,
				                                                        revInfoCfgResult.RevisionInfoTimestampData);
				validityAuditStrategy.SetRevisionTimestampGetter(revisionTimestampGetter);
			}
		}

		public GlobalConfiguration GlobalCfg { get; private set; }
		public AuditEntitiesConfiguration AuditEntCfg { get; private set; }
		public AuditProcessManager AuditProcessManager { get; private set; }
		public EntitiesConfigurations EntCfg { get; private set; }
		public RevisionInfoQueryCreator RevisionInfoQueryCreator { get; private set; }
		public RevisionInfoNumberReader RevisionInfoNumberReader { get; private set; }
		public IAuditStrategy AuditStrategy;


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