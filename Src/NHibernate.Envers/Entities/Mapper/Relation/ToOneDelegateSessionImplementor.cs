using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	public class ToOneDelegateSessionImplementor : AbstractDelegateSessionImplementor 
	{
		private readonly IAuditReaderImplementor versionsReader;
		private readonly System.Type entityClass; 
		private readonly object entityId;
		private readonly long revision;
		private readonly EntityConfiguration notVersionedEntityConfiguration;

		public ToOneDelegateSessionImplementor(IAuditReaderImplementor versionsReader,
											   System.Type entityClass,
											   object entityId, long revision, 
											   AuditConfiguration verCfg) 
			: base(versionsReader.SessionImplementor)
		{
			this.versionsReader = versionsReader;
			this.entityClass = entityClass;
			this.entityId = entityId;
			this.revision = revision;
			var entCfg = verCfg.EntCfg;
			notVersionedEntityConfiguration = entCfg.GetNotVersionEntityConfiguration(entityClass.FullName);
		}

		protected override object DoImmediateLoad(string entityName)
		{
			return notVersionedEntityConfiguration == null ? 
				versionsReader.Find(entityClass, entityId, revision) : SessionDelegate.ImmediateLoad(entityName, entityId);
		}
	}

}


