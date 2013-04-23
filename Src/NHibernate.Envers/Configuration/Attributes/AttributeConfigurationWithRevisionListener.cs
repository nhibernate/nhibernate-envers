using System;

namespace NHibernate.Envers.Configuration.Attributes
{
	public class AttributeConfigurationWithRevisionListener : AttributeConfiguration
	{
		private readonly IRevisionListener _revisionListener;

		public AttributeConfigurationWithRevisionListener(IRevisionListener revisionListener)
		{
			_revisionListener = revisionListener;
		}

		protected override Attribute ClassAttribute(Attribute attribute, System.Type type)
		{
			return attribute.GetType() == typeof(RevisionEntityAttribute) ? 
				new RevisionEntityAttribute {Listener = _revisionListener} : attribute;
		}
	}
}