using System;
using System.Linq.Expressions;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Entities.Mapper;

namespace NHibernate.Envers.Configuration.Fluent
{
	/// <summary>
	/// Provides a fluent interface for custom configuration of an audited
	/// entity. This is the generic version of <see cref="IFluentAudit"/>.
	/// </summary>
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
		IFluentAudit<T> SetTableInfo(string property, Action<AuditJoinTableAttribute> tableInfo);
		
		/// <summary>
		/// Sets table info (name, schema and/or catalog) on join table.
		/// </summary>
		/// <param name="property">A collection</param>
		/// <param name="tableInfo">A lambda expression that will fill an <see cref="AuditJoinTableAttribute"/></param>
		/// <returns></returns>
		IFluentAudit<T> SetTableInfo(Expression<Func<T, object>> property, Action<AuditJoinTableAttribute> tableInfo);

		/// <summary>
		/// Sets a custom <see cref="ICustomCollectionMapperFactory"/> for a collection.
		/// </summary>
		/// <typeparam name="TCustomCollectionMapper"></typeparam>
		/// <param name="property"></param>
		/// <returns></returns>
		IFluentAudit<T> SetCollectionMapper<TCustomCollectionMapper>(Expression<Func<T, object>> property) where TCustomCollectionMapper : ICustomCollectionMapperFactory;

		/// <summary>
		/// Use a pre-created <see cref="IEntityFactory"/> to instantiate this entity
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		IFluentAudit<T> UseFactory(IEntityFactory factory);

		/// <summary>
		/// Use a custom <see cref="IEntityFactory"/> to instantiate this entity
		/// </summary>
		/// <returns></returns>
		IFluentAudit<T> UseFactory<TFactory>() where TFactory : IEntityFactory;
	}

	/// <summary>
	/// Provides a fluent interface for custom configuration of an audited
	/// entity.
	/// </summary>
	public interface IFluentAudit
	{
		/// <summary>
		/// Excludes the property from being audited.
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		IFluentAudit Exclude(string property);

		/// <summary>
		/// Excludes the property from tracking changes on related entity.
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		IFluentAudit ExcludeRelationData(string property);

		/// <summary>
		/// Sets table info (name, schema and/or catalog) on a table.
		/// This will override the default strategy for this particular table/entity.
		/// </summary>
		/// <param name="tableInfo">A lambda expression that will fill an <see cref="AuditTableAttribute"/></param>
		/// <returns></returns>
		IFluentAudit SetTableInfo(Action<AuditTableAttribute> tableInfo);

		/// <summary>
		/// Sets table info (name, schema and/or catalog) on join table.
		/// </summary>
		/// <param name="property">A collection</param>
		/// <param name="tableInfo">A lambda expression that will fill an <see cref="AuditJoinTableAttribute"/></param>
		/// <returns></returns>
		IFluentAudit SetTableInfo(string property, Action<AuditJoinTableAttribute> tableInfo);

		/// <summary>
		/// Use a pre-created <see cref="IEntityFactory"/> to instantiate this entity
		/// </summary>
		/// <param name="factory"></param>
		/// <returns></returns>
		IFluentAudit UseFactory(IEntityFactory factory);

		/// <summary>
		/// Use a custom <see cref="IEntityFactory"/> to instantiate this entity
		/// </summary>
		/// <returns></returns>
		IFluentAudit UseFactory<TFactory>() where TFactory : IEntityFactory;
	}
}
