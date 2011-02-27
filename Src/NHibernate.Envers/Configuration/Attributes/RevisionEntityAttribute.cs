using System;

namespace NHibernate.Envers.Configuration.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class RevisionEntityAttribute : Attribute
	{
		public RevisionEntityAttribute()
		{
			Value = typeof (IRevisionListener);
		}

		public RevisionEntityAttribute(System.Type listener)
		{
			Value = listener;
		}

		public System.Type Value { get; set; }
	}
}
