using System;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.IdBag.ManyToMany.UniDirectional
{
	public partial class Fixture : TestBase
	{
		private Guid owningId1;
		private Guid owningId2;
		private Guid ownedId1;
		private Guid ownedId2;

		public Fixture(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var owning1 = new UniOwning();
			var owning2 = new UniOwning();
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(owning1);
				Session.Save(owning2);
				tx.Commit();
			}
			var owned1 = new UniOwned { Number = 1 };
			var owned2 = new UniOwned { Number = 2 };
			using (var tx = Session.BeginTransaction())
			{
				owning1.Referencing.Add(owned1);
				owning1.Referencing.Add(owned2);
				owning2.Referencing.Add(owned1);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				owning1.Referencing.Remove(owned1);
				tx.Commit();
			}
			owningId1 = owning1.Id;
			owningId2 = owning2.Id;
			ownedId1 = owned1.Id;
			ownedId2 = owned2.Id;
		}

		[Test]
		public void VerifyRevisionsCounts()
		{
			AuditReader().GetRevisions(typeof(UniOwning), owningId1)
				.Should().Have.SameSequenceAs(1, 2, 3);
			AuditReader().GetRevisions(typeof(UniOwning), owningId2)
				.Should().Have.SameSequenceAs(1, 2);

			AuditReader().GetRevisions(typeof(UniOwned), ownedId1)
				.Should().Have.SameSequenceAs(2);
			AuditReader().GetRevisions(typeof(UniOwned), ownedId2)
				.Should().Have.SameSequenceAs(2);
		}

		[Test]
		public void VerifyHistoryOfOwning1()
		{
			var owned1 = new UniOwned { Id = ownedId1, Number = 1 };
			var owned2 = new UniOwned { Id = ownedId2, Number = 2 };

			AuditReader().Find<UniOwning>(owningId1, 1)
				.Referencing.Should().Be.Empty();
			AuditReader().Find<UniOwning>(owningId1, 2)
				.Referencing.Should().Have.SameValuesAs(owned1, owned2);
			AuditReader().Find<UniOwning>(owningId1, 3)
				.Referencing.Should().Have.SameValuesAs(owned2);
		}

		[Test]
		public void VerifyHistoryOfOwning2()
		{
			var owned1 = new UniOwned { Id = ownedId1, Number = 1 };

			AuditReader().Find<UniOwning>(owningId2, 1)
				.Referencing.Should().Be.Empty();
			AuditReader().Find<UniOwning>(owningId2, 2)
				.Referencing.Should().Have.SameValuesAs(owned1);
		}


		[Test]
		public void CanReuseAsNormalEntity()
		{
			using (var tx = Session.BeginTransaction())
			{
				var ver3 = AuditReader().Find<UniOwning>(owningId1, 3);
				ver3.Referencing.First().Number = 5;
				Session.Merge(ver3);
				tx.Commit();
			}
			using (Session.BeginTransaction())
			{
				Session.Get<UniOwned>(ownedId2).Number.Should().Be.EqualTo(5);
			}
		}
	}
}