using System;

namespace NHibernate.Envers.Configuration.Attributes
{
	public class AttributeConfigurationWithRevisionListener : AttributeConfiguration
	{
		private readonly IRevisionListener revisionListener;

		public AttributeConfigurationWithRevisionListener(IRevisionListener revisionListener)
		{
			this.revisionListener = revisionListener;
		}

		protected override Attribute ClassAttribute(Attribute attribute, System.Type type)
		{
			return attribute.GetType().Equals(typeof(RevisionEntityAttribute)) ? 
				new RevisionEntityAttribute {Listener = revisionListener} : attribute;
		}
	}
}