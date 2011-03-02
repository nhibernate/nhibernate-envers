using System;
using System.Linq.Expressions;

namespace NHibernate.Envers.Configuration.Fluent
{
	public interface IFluentAudit<T> 
	{
		/// <summary>
		/// Excludes the property from being audited.
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		IFluentAudit<T> Exclude(Expression<Func<T, object>> property);

		/// <summary>
		/// Excludes the property from being audited.
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		IFluentAudit<T> Exclude(string property);

		/// <summary>
		/// Excludes the property from tracking changes on related entity.
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		IFluentAudit<T> ExcludeRelation(Expression<Func<T, object>> property);

		/// <summary>
		/// Excludes the property from tracking changes on related entity.
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		IFluentAudit<T> ExcludeRelation(string property);
	}
}