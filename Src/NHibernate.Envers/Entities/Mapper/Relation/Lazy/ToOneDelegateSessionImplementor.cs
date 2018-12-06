using System.Threading;
using System.Threading.Tasks;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy
{
	public class ToOneDelegateSessionImplementor : AbstractDelegateSessionImplementor
	{
		private readonly IAuditReaderImplementor _versionsReader;
		private readonly string _entityName;
		private readonly object _entityId;
		private readonly long _revision;
		private readonly bool _removed;
		private readonly AuditConfiguration _verCfg;

		public ToOneDelegateSessionImplementor(IAuditReaderImplementor versionsReader, string entityName,
			object entityId, long revision, bool removed, AuditConfiguration verCfg) : base(versionsReader.SessionImplementor)
		{
			_versionsReader = versionsReader;
			_entityName = entityName;
			_entityId = entityId;
			_revision = revision;
			_removed = removed;
			_verCfg = verCfg;
		}

		public override object ImmediateLoad(string entityName, object id)
		{
			return ToOneEntityLoader.LoadImmediate(_versionsReader, _entityName, _entityId, _revision, _removed, _verCfg);
		}

		public override Task<object> ImmediateLoadAsync(string entityName, object id, CancellationToken cancellationToken)
		{
			return Task.FromResult(ImmediateLoad(entityName, id));
		}
	}
}