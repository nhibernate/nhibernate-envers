using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Configuration.Fluent
{
	/// <summary>
	/// An <see cref="IAttributeProvider"/> to audit the full class.
	/// </summary>
	/// <seealso cref="IFluentAudit{T}"/>
	/// <seealso cref="LooselyTypedFluentAudit"/>
	public class FluentAudit<T> : IFluentAudit<T>, IAttributeProvider
	{
		private readonly ICollection<MemberInfoAndAttribute> attributes;
		private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

		public FluentAudit()
		{
			attributes = new List<MemberInfoAndAttribute>{new MemberInfoAndAttribute(typeof(T), new AuditedAttribute())};
		}

		public IFluentAudit<T> Exclude(Expression<Func<T, object>> property)
		{
			var methodInfo = property.MethodInfo();
			attributes.Add(new MemberInfoAndAttribute(typeof(T), methodInfo, new NotAuditedAttribute()));
			return this;
		}

		public IFluentAudit<T> Exclude(string property)
		{
			var member = getMemberOrThrow(typeof(T), property);
			attributes.Add(new MemberInfoAndAttribute(typeof(T), member, new NotAuditedAttribute()));
			return this;
		}

		public IFluentAudit<T> ExcludeRelationData(Expression<Func<T, object>> property)
		{
			var methodInfo = property.MethodInfo();
			var attr = new AuditedAttribute {TargetAuditMode = RelationTargetAuditMode.NotAudited};
			attributes.Add(new MemberInfoAndAttribute(typeof(T), methodInfo, attr));
			return this;
		}

		public IFluentAudit<T> ExcludeRelationData(string property)
		{
			var member = getMemberOrThrow(typeof(T), property);
			var attr = new AuditedAttribute { TargetAuditMode = RelationTargetAuditMode.NotAudited };
			attributes.Add(new MemberInfoAndAttribute(typeof(T), member, attr));
			return this;
		}

		public IFluentAudit<T> SetTableInfo(Action<AuditTableAttribute> tableInfo)
		{
			var attr = new AuditTableAttribute(string.Empty);
			tableInfo(attr);
			attributes.Add(new MemberInfoAndAttribute(typeof(T), attr));
			return this;
		}

		public IFluentAudit<T> SetTableInfo(Expression<Func<T, object>> property, Action<AuditJoinTableAttribute> tableInfo)
		{
			var methodInfo = property.MethodInfo();
			var attr = new AuditJoinTableAttribute();
			tableInfo(attr);
			attributes.Add(new MemberInfoAndAttribute(typeof(T), methodInfo, attr));
			return this;
		}

		private static MemberInfo getMemberOrThrow(System.Type entityType, string propertyName)
		{
			var member = entityType.GetField(propertyName, bindingFlags) ?? entityType.GetProperty(propertyName, bindingFlags) as MemberInfo;
			if (member == null)
			{
				var baseType = entityType.BaseType;
				if (baseType != null && !baseType.Equals(typeof(object)))
					return getMemberOrThrow(baseType, propertyName);
				throw new FluentException("Cannot find member " + propertyName + " on type " + entityType);
			}
			return member;
		}

		public IEnumerable<MemberInfoAndAttribute> Attributes(Cfg.Configuration nhConfiguration)
		{
			return attributes;
		}
	}
}