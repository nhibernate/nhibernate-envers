using System;

namespace NHibernate.Envers.Configuration.Attributes
{
	/// <summary>
	/// Marks an entity to be created whenever a new revision is generated. The revisions entity must have
	/// an integer-valued unique property (preferrably the primary id) annotated with <see cref="RevisionNumberAttribute"/>
	/// and a long-valued property annotated with <see cref="RevisionTimestampAttribute"/>. 
	/// The <see cref="DefaultRevisionEntity"/> already has those two fields, so you may extend it, 
	/// but you may also write your own revision entity from scratch.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class RevisionEntityAttribute : Attribute
	{
		private readonly System.Type _listenerType;
		private IRevisionListener _listener;

		public RevisionEntityAttribute()
		{
		}

		public RevisionEntityAttribute(System.Type listenerType)
		{
			_listenerType = listenerType;
		}

		public IRevisionListener Listener
		{
			get
			{
				if (_listener == null && _listenerType != null)
				{
					initListenerFromListenerType();
				}
				return _listener;
			}
			set
			{
				_listener = value;
			}
		}

		private void initListenerFromListenerType()
		{
			try
			{
				_listener = (IRevisionListener) Activator.CreateInstance(_listenerType);
			}
			catch (MissingMethodException)
			{
				throw new MappingException(string.Format("Revision listener must be of type IRevisionListener (but is: {0})", _listenerType));
			}
		}
	}
}
