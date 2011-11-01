using System.Collections.Generic;
using System.Reflection;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Configuration.Fluent
{
	public class FluentRevision : IAttributeProvider
	{
		private readonly MemberInfo _number;
		private readonly MemberInfo _timestamp;
		private readonly IRevisionListener _revisionListener;

		public FluentRevision(System.Type revisionEntityType,
										  MemberInfo number,
										  MemberInfo timestamp,
										  IRevisionListener revisionListener)
		{
			RevisionEntityType = revisionEntityType;
			_number = number;
			_timestamp = timestamp;
			_revisionListener = revisionListener;
		}

		public System.Type RevisionEntityType { get; private set; }
		
		public IEnumerable<MemberInfoAndAttribute> Attributes()
		{
			return new[]
							{
								new MemberInfoAndAttribute(RevisionEntityType, new RevisionEntityAttribute {Listener = _revisionListener}), 
								new MemberInfoAndAttribute(RevisionEntityType, _number, new RevisionNumberAttribute()),
								new MemberInfoAndAttribute(RevisionEntityType, _timestamp, new RevisionTimestampAttribute())
							};
		}
	}
}