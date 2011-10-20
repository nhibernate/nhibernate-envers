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
		private readonly ICollection<Attribute> classAttributes;
		private readonly ICollection<MemberInfoAndAttribute> memberAttributes;
		private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

		public FluentAudit()
		{
			classAttributes = new List<Attribute> {new AuditedAttribute()};
			memberAttributes = new List<MemberInfoAndAttribute>();
		}

		public IFluentAudit<T> Exclude(Expression<Func<T, object>> property)
		{
			var methodInfo = property.MethodInfo("exclusion");
			memberAttributes.Add(new MemberInfoAndAttribute(typeof(T).GetProperty(methodInfo.Name), new NotAuditedAttribute()));
			return this;
		}

		public IFluentAudit<T> Exclude(string property)
		{
			var member = getMemberOrThrow(typeof(T), property);
			memberAttributes.Add(new MemberInfoAndAttribute(member, new NotAuditedAttribute()));
			return this;
		}

		public IFluentAudit<T> ExcludeRelationData(Expression<Func<T, object>> property)
		{
			var methodInfo = property.MethodInfo("relation exclusion");
			var attr = new AuditedAttribute {TargetAuditMode = RelationTargetAuditMode.NotAudited};
			memberAttributes.Add(new MemberInfoAndAttribute(typeof(T).GetProperty(methodInfo.Name), attr));
			return this;
		}

		public IFluentAudit<T> ExcludeRelationData(string property)
		{
			var member = getMemberOrThrow(typeof(T), property);
			var attr = new AuditedAttribute { TargetAuditMode = RelationTargetAuditMode.NotAudited };
			memberAttributes.Add(new MemberInfoAndAttribute(member, attr));
			return this;
		}

		public IFluentAudit<T> SetTableInfo(Action<AuditTableAttribute> tableInfo)
		{
			var attr = new AuditTableAttribute(string.Empty);
			tableInfo(attr);
			classAttributes.Add(attr);
			return this;
		}

		public IFluentAudit<T> SetTableInfo(Expression<Func<T, object>> property, Action<AuditJoinTableAttribute> tableInfo)
		{
			var methodInfo = property.MethodInfo("table info");
			var attr = new AuditJoinTableAttribute();
			tableInfo(attr);
			memberAttributes.Add(new MemberInfoAndAttribute(methodInfo, attr));
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

		public System.Type Type
		{
			get { return typeof(T); }
		}

		public IEnumerable<Attribute> CreateClassAttributes()
		{
			return classAttributes;
		}

		public IEnumerable<MemberInfoAndAttribute> CreateMemberAttributes()
		{
			return memberAttributes;
		}
	}
}