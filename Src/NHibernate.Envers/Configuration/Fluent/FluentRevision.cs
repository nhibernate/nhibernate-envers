using System;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Configuration.Fluent
{
	public class FluentRevision : IAttributeProvider
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

		public System.Type Type
		{
			get { return _type; }
		}

		public IEnumerable<Attribute> CreateClassAttributes()
		{
			return new[] { new RevisionEntityAttribute() };
		}

		public IEnumerable<MemberInfoAndAttribute> CreateMemberAttributes()
		{
			return new[]
			          	{
			          		new MemberInfoAndAttribute(_number, new RevisionNumberAttribute()),
							new MemberInfoAndAttribute(_timestamp, new RevisionTimestampAttribute()), 
			          	};

		}
	}
}