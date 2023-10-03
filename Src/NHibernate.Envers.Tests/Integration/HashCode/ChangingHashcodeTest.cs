using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.HashCode
{
	public partial class ChangingHashcodeTest : TestBase
	{
		private int pageId;
		private int imageId;

		public ChangingHashcodeTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var page = new WikiPage {Title = "title", Content = "content"};
			var image = new WikiImage {Name = "name1"};

			using (var tx = Session.BeginTransaction())
			{
				pageId = (int) Session.Save(page);
				tx.Commit();
			} 
			using (var tx = Session.BeginTransaction())
			{
				imageId = (int) Session.Save(image);
				page.Images.Add(image);
				tx.Commit();
			} 
			using (var tx = Session.BeginTransaction())
			{
				image.Name = "name2";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(WikiPage), pageId));
			CollectionAssert.AreEquivalent(new[] { 2, 3 }, AuditReader().GetRevisions(typeof(WikiImage), imageId));
		}

		[Test]
		public void VerifyHistoryOfImage()
		{
			Assert.IsNull(AuditReader().Find<WikiImage>(imageId, 1));
			Assert.AreEqual(new WikiImage{Name="name1"}, AuditReader().Find<WikiImage>(imageId, 2));
			Assert.AreEqual(new WikiImage{Name="name2"}, AuditReader().Find<WikiImage>(imageId, 3));
		}

		[Test]
		public void VerifyHistoryOfPage()
		{
			CollectionAssert.IsEmpty(AuditReader().Find<WikiPage>(pageId, 1).Images);
			CollectionAssert.AreEquivalent(AuditReader().Find<WikiPage>(pageId, 2).Images, new[]{new WikiImage{Name="name1"}});
			CollectionAssert.AreEquivalent(AuditReader().Find<WikiPage>(pageId, 3).Images, new[]{new WikiImage{Name="name2"}});
		}
	}
}