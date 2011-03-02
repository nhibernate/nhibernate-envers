using System;

namespace NHibernate.Envers.Configuration.Attributes
{
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class RevisionEntityAttribute : Attribute
	{
		public RevisionEntityAttribute()
		{
			Listener = typeof (IRevisionListener);
		}

		public RevisionEntityAttribute(System.Type listener)
		{
			Listener = listener;
		}

		public System.Type Listener { get; set; }
	}
}
