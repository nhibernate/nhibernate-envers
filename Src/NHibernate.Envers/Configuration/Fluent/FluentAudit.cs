using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Envers.Configuration.Fluent
{
	/// <summary>
	/// An <see cref="IAttributeProvider"/> to audit the full class.
	/// </summary>
	/// <seealso cref="IFluentAudit{T}"/>
	/// <seealso cref="LooselyTypedFluentAudit"/>
	public class FluentAudit<T> : IFluentAudit<T>
	{
		private readonly ICollection<MemberInfo> excluded;
		private readonly ICollection<MemberInfo> excludedRelations;
		private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

		public FluentAudit()
		{
			excluded = new HashSet<MemberInfo>();
			excludedRelations = new HashSet<MemberInfo>();
		}

		public IFluentAudit<T> Exclude(Expression<Func<T, object>> property)
		{
			var methodInfo = property.Body.MethodInfo("exclusion"); 
			excluded.Add(typeof(T).GetProperty(methodInfo.Name));
			return this;
		}

		public IFluentAudit<T> Exclude(string property)
		{
			var member = getMemberOrThrow(typeof(T), property);
			excluded.Add(member);
			return this;
		}

		public IFluentAudit<T> ExcludeRelation(Expression<Func<T, object>> property)
		{
			var methodInfo = property.Body.MethodInfo("relation exclusion");
			excludedRelations.Add(typeof(T).GetProperty(methodInfo.Name));
			return this;
		}

		public IFluentAudit<T> ExcludeRelation(string property)
		{
			var member = getMemberOrThrow(typeof(T), property);
			excludedRelations.Add(member);
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
			return new[] {new AuditedAttribute()};
		}

		public IEnumerable<MemberInfoAndAttribute> CreateMemberAttributes()
		{
			var ret = new List<MemberInfoAndAttribute>();
			foreach (var ex in excluded)
			{
				ret.Add(new MemberInfoAndAttribute(ex, new NotAuditedAttribute()));
			}
			foreach (var ex in excludedRelations)
			{
				var attr = new AuditedAttribute
				{
					TargetAuditMode = RelationTargetAuditMode.NotAudited
				};
				ret.Add(new MemberInfoAndAttribute(ex, attr));
			}
			return ret;
		}
	}
}