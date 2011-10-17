namespace NHibernate.Envers.Tests.Entities.RevEntity.TrackModifiedEntities
{
	public class ExtendedRevisionListener : IRevisionListener
	{
		public const string CommentValue = "Comment";

		public void NewRevision(object revisionEntity)
		{
			((ExtendedRevisionEntity) revisionEntity).Comment = CommentValue;
		}
	}
}