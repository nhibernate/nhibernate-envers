namespace NHibernate.Envers.Tests.Entities.RevEntity
{
	public class TestRevisionListener : IRevisionListener
	{
		public static string Data = "data0";

		public void NewRevision(object revisionEntity)
		{
			((ListenerRevEntity)revisionEntity).Data = Data;
		}
	}
}