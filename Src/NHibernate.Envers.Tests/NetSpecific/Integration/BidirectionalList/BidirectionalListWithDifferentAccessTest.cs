using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.BidirectionalList
{
	public class BidirectionalListWithDifferentAccessTest : TestBase
	{
		private int parent_id;
		private int child1_id;
		private int child2_id;

		public BidirectionalListWithDifferentAccessTest(string strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var parent = new Parent();
			var child1 = new Child {Parent = parent};
			var child2 = new Child { Parent = parent };
			using(var tx = Session.BeginTransaction())
			{
				parent_id = (int) Session.Save(parent);
				parent.Children.Add(child1);
				parent.Children.Add(child2);
				child1_id = (int)Session.Save(child1);
				child2_id = (int)Session.Save(child2);
				tx.Commit();
			}
			using(var tx =Session.BeginTransaction())
			{
				parent.Children.RemoveAt(0);
				parent.Children.Add(child1);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(Parent), parent_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(Child), child1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(Child), child2_id));
		}

		[Test]
		public void VerifyHistoryOfParent1()
		{
			var child1 = new Child {Id = child1_id};
			var child2 = new Child {Id = child2_id};

			var ver1 = AuditReader().Find<Parent>(parent_id, 1);
			ver1.Children[0].Should().Be.EqualTo(child1);
			ver1.Children[1].Should().Be.EqualTo(child2);
		}

		[Test]
		public void VerifyHistoryOfParent2()
		{
			var child1 = new Child { Id = child1_id };
			var child2 = new Child { Id = child2_id };

			var ver1 = AuditReader().Find<Parent>(parent_id, 2);
			ver1.Children[1].Should().Be.EqualTo(child1);
			ver1.Children[0].Should().Be.EqualTo(child2);
		}
	}
}