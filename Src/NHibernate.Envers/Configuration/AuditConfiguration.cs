using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using NHibernate.Envers.Entities;
using NHibernate.Envers.RevisionInfo;
using NHibernate.Envers.Synchronization;

namespace NHibernate.Envers.Configuration
{
    /**
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public class AuditConfiguration 
    {
        public virtual GlobalConfiguration GlobalCfg { get; private set; }
        public virtual AuditEntitiesConfiguration AuditEntCfg { get; private set; }
        public virtual AuditSyncManager AuditSyncManager { get; private set; }
        public virtual EntitiesConfigurations EntCfg { get; private set; }
        public virtual RevisionInfoQueryCreator RevisionInfoQueryCreator { get; private set; }
        public virtual RevisionInfoNumberReader RevisionInfoNumberReader { get; private set; }

        //public AuditEntitiesConfiguration getAuditEntCfg() {
        //    return auditEntCfg;
        //}

        //public AuditSyncManager getSyncManager() {
        //    return auditSyncManager;
        //}

        //public GlobalConfiguration getGlobalCfg() {
        //    return globalCfg;
        //}

        //public EntitiesConfigurations getEntCfg() {
        //    return entCfg;
        //}

        //public RevisionInfoQueryCreator getRevisionInfoQueryCreator() {
        //    return revisionInfoQueryCreator;
        //}

        //public RevisionInfoNumberReader getRevisionInfoNumberReader() {
        //    return revisionInfoNumberReader;
        //}

        //TODO Simon @SuppressWarnings({"unchecked"})

        public AuditConfiguration(NHibernate.Cfg.Configuration cfg) {
            IDictionary<string,string> properties = cfg.Properties;

            //ReflectionManager reflectionManager = ((AnnotationConfiguration) cfg).getReflectionManager();
            RevisionInfoConfiguration revInfoCfg = new RevisionInfoConfiguration();
            RevisionInfoConfigurationResult revInfoCfgResult = revInfoCfg.Configure(cfg);
            AuditEntCfg = new AuditEntitiesConfiguration(properties, revInfoCfgResult.RevisionInfoEntityName);
            GlobalCfg = new GlobalConfiguration(properties);
            AuditSyncManager = new AuditSyncManager(revInfoCfgResult.RevisionInfoGenerator);
            RevisionInfoQueryCreator = revInfoCfgResult.RevisionInfoQueryCreator;
            RevisionInfoNumberReader = revInfoCfgResult.RevisionInfoNumberReader;
            EntCfg = new EntitiesConfigurator().Configure(cfg, GlobalCfg, AuditEntCfg,
                    revInfoCfgResult.RevisionInfoXmlMapping, revInfoCfgResult.RevisionInfoRelationMapping);
        }

        //

        private static IDictionary<Cfg.Configuration, AuditConfiguration> cfgs
                = new Dictionary<Cfg.Configuration, AuditConfiguration>();
                //ORIG: = new WeakHashMap<Cfg.Configuration, AuditConfiguration>(); TODO Simon see if it's ok

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static AuditConfiguration getFor(Cfg.Configuration cfg) {
            AuditConfiguration verCfg = null;
            if(cfgs.Keys.Contains(cfg))
                verCfg = cfgs[cfg];
            else{
                verCfg = new AuditConfiguration(cfg);
                cfgs.Add(cfg, verCfg);
                
                cfg.BuildMappings();
            }

            return verCfg;
        }
    }
}
