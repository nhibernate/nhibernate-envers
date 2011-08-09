using System;
using System.Collections.Generic;
using System.Reflection;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Configuration.Fluent
{
	public class FluentRevision : IAttributeProvider
	{
		private readonly System.Type _revisionEntityType;
		private readonly MemberInfo _number;
		private readonly MemberInfo _timestamp;
		private readonly System.Type _revisionListenerType;

		public FluentRevision(System.Type revisionEntityType,
										  MemberInfo number,
										  MemberInfo timestamp,
										  System.Type revisionListenerType)
		{
			_revisionEntityType = revisionEntityType;
			_number = number;
			_timestamp = timestamp;
			listenerTypeMustBeNullOfRevisionListenerType(revisionListenerType);
			_revisionListenerType = revisionListenerType;
		}

		public System.Type Type
		{
			get { return _revisionEntityType; }
		}

		public IEnumerable<Attribute> CreateClassAttributes()
		{
			var revEntityAttribute = new RevisionEntityAttribute();
			if (_revisionListenerType != null)
				revEntityAttribute.Listener = _revisionListenerType;
			return new[] { revEntityAttribute };
		}

		public IEnumerable<MemberInfoAndAttribute> CreateMemberAttributes()
		{
			return new[]
							{
								new MemberInfoAndAttribute(_number, new RevisionNumberAttribute()),
								new MemberInfoAndAttribute(_timestamp, new RevisionTimestampAttribute()), 
							};

		}

		private static void listenerTypeMustBeNullOfRevisionListenerType(System.Type revisionListenerType)
		{
			if (revisionListenerType != null && !typeof(IRevisionListener).IsAssignableFrom(revisionListenerType))
				throw new FluentException("Specified revisionListenerType does not implement IRevisionListener!");
		}
	}
}