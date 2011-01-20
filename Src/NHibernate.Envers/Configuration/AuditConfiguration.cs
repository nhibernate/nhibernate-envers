using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NHibernate.Envers.Configuration.Metadata.Reader;
using NHibernate.Envers.Entities;
using NHibernate.Envers.RevisionInfo;
using NHibernate.Envers.Synchronization;

namespace NHibernate.Envers.Configuration
{
	public class AuditConfiguration 
	{
		public AuditConfiguration(Cfg.Configuration cfg) 
		{
			var properties = cfg.Properties;
			var propertyAndMemberInfo = new PropertyAndMemberInfo();
			var revInfoCfg = new RevisionInfoConfiguration(propertyAndMemberInfo); //rk - when configure other than attr, check here
			var revInfoCfgResult = revInfoCfg.Configure(cfg); //rk - when configure other than attr, check here
			AuditEntCfg = new AuditEntitiesConfiguration(properties, revInfoCfgResult.RevisionInfoEntityName);
			GlobalCfg = new GlobalConfiguration(properties);
			AuditSyncManager = new AuditSyncManager(revInfoCfgResult.RevisionInfoGenerator);
			RevisionInfoQueryCreator = revInfoCfgResult.RevisionInfoQueryCreator;
			RevisionInfoNumberReader = revInfoCfgResult.RevisionInfoNumberReader;
			EntCfg = new EntitiesConfigurator().Configure(cfg, GlobalCfg, AuditEntCfg, //rk - when configure other than attr, check here (annotationmetadatareader & auditedpropertiesreader)
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
				//ORIG: = new WeakHashMap<Cfg.Configuration, AuditConfiguration>(); TODO Simon see if it's ok

		[MethodImpl(MethodImplOptions.Synchronized)]
		public static AuditConfiguration getFor(Cfg.Configuration cfg) 
		{
			AuditConfiguration verCfg;
			if (!cfgs.TryGetValue(cfg, out verCfg))
			{
				verCfg = new AuditConfiguration(cfg);
				cfgs.Add(cfg, verCfg);
				
				cfg.BuildMappings();
			}

			return verCfg;
		}
	}
}
