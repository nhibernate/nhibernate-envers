using System.Reflection;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;
using NHibernate.Proxy.DynamicProxy;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy
{
	public class SessionImplToOneInterceptor : NHibernate.Proxy.DynamicProxy.IInterceptor
	{
		private readonly IAuditReaderImplementor _versionsReader;
		private readonly object _entityId;
		private readonly long _revision;
		private readonly bool _removed;
		private readonly EntitiesConfigurations _entCfg;
		private static readonly MethodInfo interceptMethod = typeof(ISessionImplementor).GetMethod("ImmediateLoad");
		private readonly ISessionImplementor _target;

		public SessionImplToOneInterceptor(IAuditReaderImplementor versionsReader, object entityId, long revision, bool removed, AuditConfiguration verCfg)
		{
			_versionsReader = versionsReader;
			_entityId = entityId;
			_revision = revision;
			_removed = removed;
			_entCfg = verCfg.EntCfg;
			_target = _versionsReader.SessionImplementor;
		}

		public object Intercept(InvocationInfo info)
		{
			return info.TargetMethod == interceptMethod ? 
				doImmediateLoad((string) info.Arguments[0]) : 
				info.TargetMethod.Invoke(_target, info.Arguments);
		}

		private object doImmediateLoad(string entityName)
		{
			return _entCfg.GetNotVersionEntityConfiguration(entityName) == null ?
				_versionsReader.Find(entityName, _entityId, _revision, _removed) :
				// notAudited relation, look up entity with hibernate
				_target.ImmediateLoad(entityName, _entityId);
		}
	}
}