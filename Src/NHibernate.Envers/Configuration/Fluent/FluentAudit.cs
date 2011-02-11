using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Envers.Configuration.Fluent
{
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
			excluded.Add(property.Body.MethodInfo("exclusion"));
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
			excludedRelations.Add(property.Body.MethodInfo("relation exclusion"));
			return this;
		}

		public IFluentAudit<T> ExcludeRelation(string property)
		{
			var member = getMemberOrThrow(typeof(T), property);
			excludedRelations.Add(member);
			return this;
		}

		public IDictionary<MemberInfo, IEnumerable<Attribute>> Create()
		{
			var ret = new Dictionary<MemberInfo, IEnumerable<Attribute>>();
			
			addType(ret, typeof(T));
			addExcludedInfo(ret);
			addExcludedRelationInfo(ret);
			return ret;
		}

		private void addExcludedRelationInfo(IDictionary<MemberInfo, IEnumerable<Attribute>> ret)
		{
			foreach (var ex in excludedRelations)
			{
				var attr = new AuditedAttribute
				           	{
				           		TargetAuditMode = RelationTargetAuditMode.NotAudited
				           	};
				ret[ex] = new List<Attribute> { attr };
			}
		}

		private void addExcludedInfo(IDictionary<MemberInfo, IEnumerable<Attribute>> ret)
		{
			foreach (var ex in excluded)
			{
				ret[ex] = new List<Attribute> {new NotAuditedAttribute()};
			}
		}

		private static void addType(IDictionary<MemberInfo, IEnumerable<Attribute>> ret, System.Type type)
		{
			ret[type] = new List<Attribute> {new AuditedAttribute()};
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
	}
}