using System;
using System.Linq.Expressions;

namespace NHibernate.Envers.Configuration.Fluent
{
	public interface IFluentAudit<T> : IAttributeProvider
	{
		IFluentAudit<T> Exclude(Expression<Func<T, object>> property);
		IFluentAudit<T> Exclude(string property);
		IFluentAudit<T> ExcludeRelation(Expression<Func<T, object>> property);
		IFluentAudit<T> ExcludeRelation(string property);
	}
}