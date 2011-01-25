using System;

namespace NHibernate.Envers.Tests.Entities.RevEntity
{
	public class TestExceptionRevisionListener : IRevisionListener
	{
		public void NewRevision(object revisionEntity)
		{
			throw new Exception();
		}
	}
}