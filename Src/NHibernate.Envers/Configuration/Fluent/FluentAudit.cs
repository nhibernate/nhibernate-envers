using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Envers.Configuration.Fluent
{
	public class FluentAudit<T> : IFluentAudit<T>
	{
		private readonly ICollection<MemberInfo> excluded;
		private const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

		public FluentAudit()
		{
			excluded = new HashSet<MemberInfo>();
		}

		public IFluentAudit<T> Exclude(Expression<Func<T, object>> property)
		{
			excluded.Add(property.Body.MethodInfo("exclusion"));
			return this;
		}

		public IFluentAudit<T> Exclude(string propertyName)
		{
			var entityType = typeof (T);
			var member = entityType.GetField(propertyName, bindingFlags) ?? entityType.GetProperty(propertyName, bindingFlags) as MemberInfo;
			if(member==null)	
				throw new FluentException("Cannot find member " + propertyName + " on type " + entityType);
			excluded.Add(member);
			return this;
		}

		public IDictionary<MemberInfo, IEnumerable<Attribute>> Create()
		{
			var ret = new Dictionary<MemberInfo, IEnumerable<Attribute>>();
			
			addType(ret, typeof(T));
			foreach (var ex in excluded)
			{
				ret[ex] = new List<Attribute> {new NotAuditedAttribute()};
			}
			return ret;
		}

		private static void addType(IDictionary<MemberInfo, IEnumerable<Attribute>> ret, System.Type type)
		{
			ret[type] = new List<Attribute> {new AuditedAttribute()};
		}
	}
}