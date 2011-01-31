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
		public AuditConfiguration(Cfg.Configuration cfg, IMetaDataProvider metaDataProvider) 
		{
			var mds = new MetaDataStore(cfg, metaDataProvider);

			var properties = cfg.Properties;
			var propertyAndMemberInfo = new PropertyAndMemberInfo();
			var revInfoCfg = new RevisionInfoConfiguration(mds, propertyAndMemberInfo); //rk - when configure other than attr, check here
			var revInfoCfgResult = revInfoCfg.Configure(cfg); //rk - when configure other than attr, check here
			AuditEntCfg = new AuditEntitiesConfiguration(properties, revInfoCfgResult.RevisionInfoEntityName);
			GlobalCfg = new GlobalConfiguration(properties);
			AuditSyncManager = new AuditSyncManager(revInfoCfgResult.RevisionInfoGenerator);
			RevisionInfoQueryCreator = revInfoCfgResult.RevisionInfoQueryCreator;
			RevisionInfoNumberReader = revInfoCfgResult.RevisionInfoNumberReader;
			EntCfg = new EntitiesConfigurator().Configure(cfg, mds, GlobalCfg, AuditEntCfg, //rk - when configure other than attr, check here (annotationmetadatareader & auditedpropertiesreader)
					revInfoCfgResult.RevisionInfoXmlMapping, revInfoCfgResult.RevisionInfoRelationMapping, propertyAndMemberInfo);
		}

		public virtual GlobalConfiguration GlobalCfg { get; private set; }
		public virtual AuditEntitiesConfiguration AuditEntCfg { get; private set; }
		public virtual AuditSyncManager AuditSyncManager { get; private set; }
		public virtual EntitiesConfigurations EntCfg { get; private set; }
		public virtual RevisionInfoQueryCreator RevisionInfoQueryCreator { get; private set; }
		public virtual RevisionInfoNumberReader RevisionInfoNumberReader { get; private set; }

		private static readonly IDictionary<Cfg.Configuration, AuditConfiguration> cfgs
				= new Dictionary<Cfg.Configuration, AuditConfiguration>();

		private static readonly IDictionary<Cfg.Configuration, IMetaDataProvider> confMetas
				= new Dictionary<Cfg.Configuration, IMetaDataProvider>();


		[MethodImpl(MethodImplOptions.Synchronized)]
		public static AuditConfiguration GetFor(Cfg.Configuration cfg) 
		{
			AuditConfiguration verCfg;
			if (!cfgs.TryGetValue(cfg, out verCfg))
			{
				IMetaDataProvider metas;
				if (!confMetas.TryGetValue(cfg, out metas))
					metas = new AttributeMetaDataProvider();
				verCfg = new AuditConfiguration(cfg, metas);
				cfgs.Add(cfg, verCfg);
				
				cfg.BuildMappings();
			}

			return verCfg;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static void SetConfigMetas(Cfg.Configuration cfg, IMetaDataProvider metaDataProvider)
		{
			confMetas[cfg] = metaDataProvider;
		}
	}
}
