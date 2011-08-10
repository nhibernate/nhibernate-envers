using System;
using System.Collections.Generic;

namespace NHibernate.Envers.Configuration.Attributes
{
	public class AttributeConfigurationWithRevisionListener : AttributeConfiguration
	{
		private readonly IRevisionListener revisionListener;

		public AttributeConfigurationWithRevisionListener(IRevisionListener revisionListener)
		{
			this.revisionListener = revisionListener;
		}

		protected override void AddClassAttribute(IDictionary<System.Type, Store.IEntityMeta> dictionaryToFill, 
																Attribute attribute, 
																System.Type type)
		{
			if (attribute.GetType().Equals(typeof(RevisionEntityAttribute)))
			{
				var replacedAttribute = new RevisionEntityAttribute {Listener = revisionListener};
				base.AddClassAttribute(dictionaryToFill, replacedAttribute, type);
			}
			else
			{
				base.AddClassAttribute(dictionaryToFill, attribute, type);				
			}
		}
	}
}