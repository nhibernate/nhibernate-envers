using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace NHibernate.Envers.Configuration.Fluent
{
	public class FluentAudit<T> : IFluentAudit<T>
	{
		private readonly ICollection<MemberInfo> excluded;

		public FluentAudit()
		{
			excluded = new HashSet<MemberInfo>();
		}

		public IFluentAudit<T> Exclude(Expression<Func<T, object>> func)
		{
			excluded.Add(func.Body.MethodInfo("exclusion"));
			return this;
		}

		public IDictionary<MemberInfo, IEnumerable<Attribute>> Create()
		{
			var ret = new Dictionary<MemberInfo, IEnumerable<Attribute>>();
			ret[typeof (T)] = new List<Attribute> {new AuditedAttribute()};
			foreach (var ex in excluded)
			{
				ret[ex] = new List<Attribute> {new NotAuditedAttribute()};
			}
			return ret;
		}
	}
}