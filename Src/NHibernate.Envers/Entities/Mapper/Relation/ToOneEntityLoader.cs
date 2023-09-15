using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	public static partial class ToOneEntityLoader
	{
		public static object LoadImmediate(IAuditReaderImplementor versionsReader, string entityName,
			object entityId, long revision, bool removed, AuditConfiguration verCfg)
		{
			if (verCfg.EntCfg.GetNotVersionEntityConfiguration(entityName) == null)
			{
				// Audited relation, look up entity with Envers.
				// When user traverses removed entities graph, do not restrict revision type of referencing objects
				// to ADD or MOD (DEL possible). See HHH-5845.
				return versionsReader.Find(entityName, entityId, revision, removed);
			}
			// Not audited relation, look up entity with Hibernate.
			return versionsReader.SessionImplementor.ImmediateLoad(entityName, entityId);
		}

		public static object CreateProxyOrLoadImmediate(IAuditReaderImplementor versionsReader, string entityName,
			object entityId, long revision, bool removed, AuditConfiguration verCfg)
		{
			var persister = versionsReader.SessionImplementor.Factory.GetEntityPersister(entityName);
			if (persister.HasProxy)
			{
				return persister.CreateProxy(entityId,
					new ToOneDelegateSessionImplementor(versionsReader, entityName, entityId, revision, removed, verCfg));
			}

			return LoadImmediate(versionsReader, entityName, entityId, revision, removed, verCfg);
		}
	}
}