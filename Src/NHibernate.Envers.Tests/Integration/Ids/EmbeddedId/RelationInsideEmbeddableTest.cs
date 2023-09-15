using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Ids.EmbeddedId
{
	public partial class RelationInsideEmbeddableTest : TestBase
	{
		private int orderId;
		private ItemId itemId;

		public RelationInsideEmbeddableTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var producer = new Producer {Id = 1, Name = "Sony"};
			itemId = new ItemId {Model = "TV", Producer = producer, Version = 1};
			var item = new Item {Id = itemId, Price = 100.5};
			var order = new PurchaseOrder {Item = item};

			//rev 1
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(producer);
				Session.Save(item);
				orderId = (int) Session.Save(order);
				tx.Commit();
			}

			//rev 2
			using (var tx = Session.BeginTransaction())
			{
				order.Comment = "fragile";
				tx.Commit();
			}

			//rev 3
			using (var tx = Session.BeginTransaction())
			{
				item.Price = 110;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionsCount()
		{
			AuditReader().GetRevisions(typeof (PurchaseOrder), orderId)
				.Should().Have.SameSequenceAs(1, 2);
			AuditReader().GetRevisions(typeof (Item), itemId)
				.Should().Have.SameSequenceAs(1, 3);
		}

		[Test]
		public void VerifyHistoryOfPurchaseOrder()
		{
			var item = new Item
			           	{
			           		Id = new ItemId {Model = "TV", Producer = new Producer {Id = 1, Name = "Sony"}, Version = 1},
			           		Price = 100.5
			           	};
			AuditReader().Find<PurchaseOrder>(orderId, 1)
				.Should().Be.EqualTo(new PurchaseOrder {Id = orderId, Item = item});
			AuditReader().Find<PurchaseOrder>(orderId, 2)
				.Should().Be.EqualTo(new PurchaseOrder {Id = orderId, Item = item, Comment = "fragile"});
		}

		[Test]
		public void VerifyHistoryOfItem()
		{
			AuditReader().Find<Item>(itemId, 1)
				.Should().Be.EqualTo(new Item {Id = itemId, Price = 100.5});
			AuditReader().Find<Item>(itemId, 3)
				.Should().Be.EqualTo(new Item { Id = itemId, Price = 110 });
		} 
	}
}