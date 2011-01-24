using System;

namespace NHibernate.Envers
{
	[AttributeUsage(AttributeTargets.Class)]
	public class RevisionEntityAttribute : Attribute
	{
		public RevisionEntityAttribute()
		{
			Value = typeof (IRevisionListener);
		}

		public System.Type Value { get; set; }
	}
}
