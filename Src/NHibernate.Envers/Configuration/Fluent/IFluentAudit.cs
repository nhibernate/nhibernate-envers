using System;
using System.Linq.Expressions;
using NHibernate.Envers.Configuration.Attributes;

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
		IFluentAudit<T> ExcludeRelationData(Expression<Func<T, object>> property);

		/// <summary>
		/// Excludes the property from tracking changes on related entity.
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		IFluentAudit<T> ExcludeRelationData(string property);

		/// <summary>
		/// Sets table info (name, schema and/or catalog) on a table.
		/// This will override the default strategy for this particular table/entity.
		/// </summary>
		/// <param name="tableInfo">A lambda expression that will fill an <see cref="AuditTableAttribute"/></param>
		/// <returns></returns>
		IFluentAudit<T> SetTableInfo(Action<AuditTableAttribute> tableInfo);

		/// <summary>
		/// Sets table info (name, schema and/or catalog) on join table.
		/// </summary>
		/// <param name="property">A collection</param>
		/// <param name="tableInfo">A lambda expression that will fill an <see cref="AuditJoinTableAttribute"/></param>
		/// <returns></returns>
		IFluentAudit<T> SetTableInfo(Expression<Func<T, object>> property, Action<AuditJoinTableAttribute> tableInfo);
	}
}