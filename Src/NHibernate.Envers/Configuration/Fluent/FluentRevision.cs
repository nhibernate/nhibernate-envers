using System.Collections.Generic;
using System.Reflection;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Configuration.Fluent
{
	public class FluentRevision : IAttributeProvider
	{
		private readonly MemberInfo _number;
		private readonly MemberInfo _timestamp;
		private readonly MemberInfo _modifiedEntityNames;
		private readonly IRevisionListener _revisionListener;

		public FluentRevision(System.Type revisionEntityType,
											MemberInfo number,
											MemberInfo timestamp,
											MemberInfo modifiedEntityNames,
											IRevisionListener revisionListener)
		{
			RevisionEntityType = revisionEntityType;
			_number = number;
			_timestamp = timestamp;
			_modifiedEntityNames = modifiedEntityNames;
			_revisionListener = revisionListener;
		}

		public System.Type RevisionEntityType { get; private set; }

		public IEnumerable<MemberInfoAndAttribute> Attributes(Cfg.Configuration nhConfiguration)
		{
			if(nhConfiguration.GetClassMapping(RevisionEntityType)==null)
				throw new FluentException("Revision entity " + RevisionEntityType.FullName + " must be mapped!");
			var ret = new List<MemberInfoAndAttribute>
							{
								new MemberInfoAndAttribute(RevisionEntityType, new RevisionEntityAttribute {Listener = _revisionListener}), 
								new MemberInfoAndAttribute(RevisionEntityType, _number, new RevisionNumberAttribute()),
								new MemberInfoAndAttribute(RevisionEntityType, _timestamp, new RevisionTimestampAttribute())
							};
			if(_modifiedEntityNames!=null)
				ret.Add(new MemberInfoAndAttribute(RevisionEntityType, _modifiedEntityNames, new ModifiedEntityNamesAttribute()));
			return ret;
		}
	}
}