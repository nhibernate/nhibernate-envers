using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.SuperClass
{
	[TestFixture]
	public class MappedSubclassingTest : TestBase
	{
		private int id1;
	
		protected override void Initialize()
		{
			var se1 = new SubclassEntity {Str = "x"};
			using (var tx = Session.BeginTransaction())
			{
				id1 = (int) Session.Save(se1);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				se1.Str = "y";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(SubclassEntity), id1));
		}

		[Test]
		public void VerifyHistory()
		{
			var ver1 = new SubclassEntity { Id = id1, Str = "x" };
			var ver2 = new SubclassEntity { Id = id1, Str = "y" };

			Assert.AreEqual(ver1, AuditReader().Find<SubclassEntity>(id1, 1));
			Assert.AreEqual(ver2, AuditReader().Find<SubclassEntity>(id1, 2));
		}
	}
}