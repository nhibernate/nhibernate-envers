using System;
using System.Linq.Expressions;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Entities.Mapper;

namespace NHibernate.Envers.Configuration.Fluent
{
	/// <summary>
	/// An <see cref="IAttributeProvider"/> to audit the full class.
	/// </summary>
	/// <seealso cref="IFluentAudit{T}"/>
	/// <seealso cref="LooselyTypedFluentAudit"/>
	public class FluentAudit<T> : BaseFluentAudit, IFluentAudit<T>
	{
		public FluentAudit()
		{
			AttributeCollection.Add(new MemberInfoAndAttribute(typeof(T), new AuditedAttribute()));
		}

		public IFluentAudit<T> Exclude(Expression<Func<T, object>> property)
		{
			var methodInfo = property.MethodInfo();
			AttributeCollection.Add(new MemberInfoAndAttribute(typeof(T), methodInfo, new NotAuditedAttribute()));
			return this;
		}

		public IFluentAudit<T> Exclude(string property)
		{
			Exclude(typeof (T), property);
			return this;
		}

		public IFluentAudit<T> ExcludeRelationData(Expression<Func<T, object>> property)
		{
			var methodInfo = property.MethodInfo();
			var attr = new AuditedAttribute {TargetAuditMode = RelationTargetAuditMode.NotAudited};
			AttributeCollection.Add(new MemberInfoAndAttribute(typeof(T), methodInfo, attr));
			return this;
		}

		public IFluentAudit<T> ExcludeRelationData(string property)
		{
			ExcludeRelationData(typeof (T), property);
			return this;
		}

		public IFluentAudit<T> SetTableInfo(Action<AuditTableAttribute> tableInfo)
		{
			SetTableInfo(typeof (T), tableInfo);
			return this;
		}

		public IFluentAudit<T> SetTableInfo(string property, Action<AuditJoinTableAttribute> tableInfo)
		{
			SetTableInfo(typeof(T), property, tableInfo);
			return this;
		}

		public IFluentAudit<T> SetTableInfo(Expression<Func<T, object>> property, Action<AuditJoinTableAttribute> tableInfo)
		{
			var methodInfo = property.MethodInfo();
			var attr = new AuditJoinTableAttribute();
			tableInfo(attr);
			AttributeCollection.Add(new MemberInfoAndAttribute(typeof(T), methodInfo, attr));
			return this;
		}

		public IFluentAudit<T> SetCollectionMapper<TCustomCollectionMapper>(Expression<Func<T, object>> property) where TCustomCollectionMapper : ICustomCollectionMapperFactory
		{
			var methodInfo = property.MethodInfo();
			var attr = new CustomCollectionMapperAttribute(typeof (TCustomCollectionMapper));
			AttributeCollection.Add(new MemberInfoAndAttribute(typeof(T), methodInfo, attr));
			return this;
		}

		public IFluentAudit<T> UseFactory(IEntityFactory factory)
		{
			AttributeCollection.Add(new MemberInfoAndAttribute(typeof(T), new AuditFactoryAttribute(factory)));
			return this;
		}

		public IFluentAudit<T> UseFactory<TFactory>() where TFactory:IEntityFactory
		{
			AttributeCollection.Add(new MemberInfoAndAttribute(typeof(T), new AuditFactoryAttribute(typeof(TFactory))));
			return this;
		}
	}
}