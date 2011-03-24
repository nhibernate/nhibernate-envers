using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers
{
	public interface IRevisionListener
	{
		/// <summary>
		/// Called when a new revision is created.
		/// </summary>
		/// <remarks>
		/// Implementations must have a default ctor.
		/// </remarks>
		/// <param name="revisionEntity">
		/// An instance of the entity annotated with <see cref="RevisionEntityAttribute"/> which will be persisted
		/// after this method returns. All properties on this entity that are to be persisted should be set by this method.
		/// </param>
		void NewRevision(object revisionEntity);
	}
}