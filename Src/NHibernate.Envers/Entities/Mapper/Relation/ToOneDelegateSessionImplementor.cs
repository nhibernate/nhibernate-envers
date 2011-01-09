using System;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
    /**
     * @author Catalina Panait, port of Envers omonyme class by Adam Warski (adam at warski dot org)
                                                              * Tomasz Bech
     */
    public class ToOneDelegateSessionImplementor : AbstractDelegateSessionImplementor 
    {
        private readonly IAuditReaderImplementor versionsReader;
        private readonly System.Type entityClass; 
        private readonly Object entityId;
        private readonly long revision;
	    private EntityConfiguration notVersionedEntityConfiguration;

	    public ToOneDelegateSessionImplementor(IAuditReaderImplementor versionsReader,
                                               System.Type entityClass, 
                                               Object entityId, long revision, 
                                               AuditConfiguration verCfg) 
            : base(versionsReader.SessionImplementor)
        {
            this.versionsReader = versionsReader;
            this.entityClass = entityClass;
            this.entityId = entityId;
            this.revision = revision;
            EntitiesConfigurations entCfg = verCfg.EntCfg;
            notVersionedEntityConfiguration = entCfg.GetNotVersionEntityConfiguration(entityClass.FullName);
        }

        public override Object doImmediateLoad(String entityName) 
        {
		    if (notVersionedEntityConfiguration == null) 
            {
                return versionsReader.Find(entityClass, entityId, revision);
		    } 
		    return delegat.ImmediateLoad(entityName, entityId);
        }
    }

}


