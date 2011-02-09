using System;
using System.Collections.Generic;
using System.Reflection;

namespace NHibernate.Envers.Configuration.Fluent
{
	public class FluentRevision : IAttributesPerMethodInfoFactory
	{
		private readonly System.Type _type;
		private readonly MemberInfo _number;
		private readonly MemberInfo _timestamp;

		public FluentRevision(System.Type type, MemberInfo number, MemberInfo timestamp)
		{
			_type = type;
			_number = number;
			_timestamp = timestamp;
		}

		public IDictionary<MemberInfo, IEnumerable<Attribute>> Create()
		{
			var ret = new Dictionary<MemberInfo, IEnumerable<Attribute>>();
			ret[_type] = new[]{new RevisionEntityAttribute()};
			ret[_number] = new[] {new RevisionNumberAttribute()};
			ret[_timestamp] = new[] {new RevisionTimestampAttribute()};
			return ret;
		}
	}
}