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
		private readonly Cfg.Configuration _configuration; //for serialization

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
			_configuration = cfg;
		}

		public GlobalConfiguration GlobalCfg { get; }
		public AuditEntitiesConfiguration AuditEntCfg { get; }
		public AuditProcessManager AuditProcessManager { get; }
		public EntitiesConfigurations EntCfg { get; }
		public RevisionInfoQueryCreator RevisionInfoQueryCreator { get; }
		public RevisionInfoNumberReader RevisionInfoNumberReader { get; }
		public ModifiedEntityNamesReader ModifiedEntityNamesReader { get; }
		public IGetter RevisionTimestampGetter { get; }


		[MethodImpl(MethodImplOptions.Synchronized)]
		public static AuditConfiguration GetFor(Cfg.Configuration cfg)
		{
			if (!configurations.TryGetValue(cfg, out var verCfg))
			{
				cfg.SetEnversProperty(ConfigurationKey.UniqueConfigurationName, Guid.NewGuid().ToString());
				cfg.BuildMappings(); // force secondpass for mappings added by users
				if (!configurationMetadataProvider.TryGetValue(cfg, out var metas))
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
			configurations[_configuration] = this;
		}
	}
}