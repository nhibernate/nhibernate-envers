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
		private readonly IRevisionListener _revisionListener;

		public FluentRevision(System.Type revisionEntityType,
										  MemberInfo number,
										  MemberInfo timestamp,
										  IRevisionListener revisionListener)
		{
			_revisionEntityType = revisionEntityType;
			_number = number;
			_timestamp = timestamp;
			_revisionListener = revisionListener;
		}

		public System.Type Type
		{
			get { return _revisionEntityType; }
		}
		
		public IEnumerable<MemberInfoAndAttribute> Attributes()
		{
			return new[]
							{
								new MemberInfoAndAttribute(_revisionEntityType, new RevisionEntityAttribute {Listener = _revisionListener}), 
								new MemberInfoAndAttribute(_number, new RevisionNumberAttribute()),
								new MemberInfoAndAttribute(_timestamp, new RevisionTimestampAttribute())
							};

		}
	}
}