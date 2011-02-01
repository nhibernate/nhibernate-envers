using System;
using System.Linq.Expressions;

namespace NHibernate.Envers.Configuration.Fluent
{
	public interface IFluentAudit<T> : IAttributeFactory
	{
		IFluentAudit<T> Exclude(Expression<Func<T, object>> func);
	}
}