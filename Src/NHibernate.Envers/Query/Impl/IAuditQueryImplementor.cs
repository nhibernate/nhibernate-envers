using NHibernate.Envers.Query.Projection;

namespace NHibernate.Envers.Query.Impl
{
	public interface IAuditQueryImplementor : IAuditQuery
	{
		void RegisterProjection(string entityName, IAuditProjection projection);
	}
}