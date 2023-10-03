using System.Collections.Generic;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ManyToOne.BiDirectional
{
	public partial class ImplicitMappedByTest : TestBase
	{
		private long ownedId;
		private long owning1Id;
		private long owning2Id;

		public ImplicitMappedByTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var owned = new OneToManyOwned {Data = "data"};
			var referencing = new HashSet<ManyToOneOwning>();
			var owning1 = new ManyToOneOwning { Data = "data1", References = owned };
			referencing.Add(owning1);
			var owning2 = new ManyToOneOwning { Data = "data2", References = owned };
			referencing.Add(owning2);
			owned.Referencing = referencing;

			//revision 1
			using (var tx = Session.BeginTransaction())
			{
				ownedId = (long) Session.Save(owned);
				owning1Id = (long) Session.Save(owning1);
				owning2Id = (long) Session.Save(owning2);
				tx.Commit();
			}

			//Revision 2
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(owning1);
				tx.Commit();
			}

			//Revision 3
			using (var tx = Session.BeginTransaction())
			{
				owning2.Data = "data2modified";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionsCounts()
		{
			AuditReader().GetRevisions(typeof (OneToManyOwned), ownedId)
				.Should().Have.SameSequenceAs(1, 2);
			AuditReader().GetRevisions(typeof(ManyToOneOwning), owning1Id)
				.Should().Have.SameSequenceAs(1, 2);
			AuditReader().GetRevisions(typeof(ManyToOneOwning), owning2Id)
				.Should().Have.SameSequenceAs(1, 3);
		}

		[Test]
		public void VerifyHistoryOfOwned()
		{
			var owned = new OneToManyOwned {Data = "data", Id = ownedId};
			var owning1 = new ManyToOneOwning {Data = "data1", References = owned, Id = owning1Id};
			var owning2 = new ManyToOneOwning {Data = "data2", References = owned, Id = owning2Id};

			var ver1 = AuditReader().Find<OneToManyOwned>(ownedId, 1);
			ver1.Should().Be.EqualTo(owned);
			ver1.Referencing.Should().Have.SameValuesAs(owning1, owning2);

			var ver2 = AuditReader().Find<OneToManyOwned>(ownedId, 2);
			ver2.Should().Be.EqualTo(owned);
			ver2.Referencing.Should().Have.SameValuesAs(owning2);
		}

		[Test]
		public void VerifyHistoryOfOwning1()
		{
			var ver1 = new ManyToOneOwning {Data = "data1", Id = owning1Id};
			AuditReader().Find<ManyToOneOwning>(owning1Id, 1)
				.Should().Be.EqualTo(ver1);
		}

		[Test]
		public void VerifyHistoryOfOwning2()
		{
			var owned = new OneToManyOwned {Data = "data", Id = ownedId};
			var owning1 = new ManyToOneOwning {Data = "data2", Id = owning2Id, References = owned};
			var owning3 = new ManyToOneOwning {Data = "data2modified", Id = owning2Id, References = owned};

			var ver1 = AuditReader().Find<ManyToOneOwning>(owning2Id, 1);
			var ver3 = AuditReader().Find<ManyToOneOwning>(owning2Id, 3);

			ver1.Should().Be.EqualTo(owning1);
			ver1.References.Id.Should().Be.EqualTo(owned.Id);
			ver3.Should().Be.EqualTo(owning3);
			ver3.References.Id.Should().Be.EqualTo(owned.Id);
		}
	}
}