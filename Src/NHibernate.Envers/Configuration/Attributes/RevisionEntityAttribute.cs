using System;

namespace NHibernate.Envers.Configuration.Attributes
{
	/// <summary>
	/// Marks an entity to be created whenever a new revision is generated. The revisions entity must have
	/// an integer-valued unique property (preferrably the primary id) annotated with <see cref="RevisionNumberAttribute"/>
	/// and a long-valued property annotated with <see cref="RevisionTimestampAttribute"/>. 
	/// The <see cref="DefaultRevisionEntity"/> already has those two fields, so you may extend it, 
	/// but you may also write your own revision entity  from scratch.
	/// </summary>
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
