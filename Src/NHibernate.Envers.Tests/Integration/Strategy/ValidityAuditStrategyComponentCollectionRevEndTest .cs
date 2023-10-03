using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Tests.Integration.Strategy.Model;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Strategy
{
	public partial class ValidityAuditStrategyComponentCollectionRevEndTest : ValidityTestBase
	{
		private int productId;

		protected override void Initialize()
		{
			var product = new Product
			{
				Name = "Test",
				Items = new List<Item>
					{
						new Item
						{
							Name = "bread",
							Value = null
						}
					}
			};

			using (var tx = Session.BeginTransaction())
			{
				productId = (int) Session.Save(product);
				tx.Commit();
			}

			using (var tx = Session.BeginTransaction())
			{
				product.Items.Add(new Item {Name = "bread2", Value = 2});
				tx.Commit();
			}

			using (var tx = Session.BeginTransaction())
			{
				product.Items.RemoveAt(0);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			AuditReader().GetRevisions(typeof(Product), productId)
				.Should().Have.SameSequenceAs(1, 2, 3);
		}

		[Test]
		public void VerifyRevision1()
		{
			var product = AuditReader().Find<Product>(productId, 1);
			product.Items.Single().Name.Should().Be.EqualTo("bread");
		}

		[Test]
		public void VerifyRevision2()
		{
			var product = AuditReader().Find<Product>(productId, 2);
			product.Items.Select(x => x.Name)
				.Should().Have.SameSequenceAs("bread", "bread2");
		}

		[Test]
		public void VerifyRevision3()
		{
			var product = AuditReader().Find<Product>(productId, 3);
			product.Items.Single().Name.Should().Be.EqualTo("bread2");
		}

		protected override IEnumerable<string> Mappings => new[] {"Integration.Strategy.Model.Mapping.hbm.xml"};
	}
}