using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.MultipleRelations
{
	public partial class MultipleAssociationsTest : TestBase
	{
		private long lukaszId;
		private long kingaId;
		private long warsawId;
		private long cracowId;

		public MultipleAssociationsTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var lukasz = new Person { Name = "Lukasz" };
			var kinga = new Person { Name = "Kinga" };
			var warsaw = new Address { City = "Warsaw" };
			var cracow = new Address { City = "Cracow" };
			warsaw.Tenants.Add(lukasz);
			warsaw.Landlord = lukasz;
			warsaw.Tenants.Add(kinga);
			lukasz.Addresses.Add(warsaw);
			lukasz.OwnedAddresses.Add(warsaw);
			kinga.Addresses.Add(warsaw);

			//Revision 1
			using (var tx = Session.BeginTransaction())
			{
				lukaszId = (long) Session.Save(lukasz);
				kingaId = (long) Session.Save(kinga);
				warsawId = (long) Session.Save(warsaw);
				tx.Commit();
			}

			//Revision 2
			using (var tx = Session.BeginTransaction())
			{
				kinga.Addresses.Add(cracow);
				cracow.Tenants.Add(kinga);
				cracow.Landlord = kinga;
				cracowId = (long) Session.Save(cracow);
				tx.Commit();
			}

			//Revision 3
			using (var tx = Session.BeginTransaction())
			{
				cracow.City = "Krakow";
				tx.Commit();
			}

			//Revision 4
			using (var tx = Session.BeginTransaction())
			{
				lukasz.Name = "Lucas";
				tx.Commit();
			}

			//Revision 5
			using (var tx = Session.BeginTransaction())
			{
				warsaw.Landlord = kinga;
				kinga.OwnedAddresses.Add(warsaw);
				kinga.OwnedAddresses.Remove(warsaw);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionsCounts()
		{
			AuditReader().GetRevisions(typeof(Person), lukaszId)
				.Should().Have.SameSequenceAs(1, 4, 5);
			AuditReader().GetRevisions(typeof(Person), kingaId)
				.Should().Have.SameSequenceAs(1, 2, 5);
			AuditReader().GetRevisions(typeof(Address), warsawId)
				.Should().Have.SameSequenceAs(1, 5);
			AuditReader().GetRevisions(typeof(Address), cracowId)
				.Should().Have.SameSequenceAs(2, 3);
		}

		[Test]
		public void VerifyHistoryOfLukasz()
		{
			var lukasz = new Person {Id = lukaszId, Name = "Lukasz"};
			var warsaw = new Address {Id = warsawId, City = "Warsaw"};
			lukasz.Addresses.Add(warsaw);
			lukasz.OwnedAddresses.Add(warsaw);

			var ver1 = AuditReader().Find<Person>(lukaszId, 1);
			lukasz.Should().Be.EqualTo(ver1);
			lukasz.Addresses.Should().Have.SameValuesAs(ver1.Addresses);
			lukasz.OwnedAddresses.Should().Have.SameValuesAs(ver1.OwnedAddresses);

			lukasz.Name = "Lucas";

			var ver4 = AuditReader().Find<Person>(lukaszId, 4);
			lukasz.Should().Be.EqualTo(ver4);

			lukasz.OwnedAddresses.Remove(warsaw);

			var ver5 = AuditReader().Find<Person>(lukaszId, 5);
			lukasz.OwnedAddresses.Should().Have.SameValuesAs(ver5.OwnedAddresses);
		}

		[Test]
		public void VerifyHistoryOfKinga()
		{
			var kinga = new Person { Id = kingaId, Name = "Kinga" };
			var warsaw = new Address { Id = warsawId, City = "Warsaw" };
			kinga.Addresses.Add(warsaw);

			var ver1 = AuditReader().Find<Person>(kingaId, 1);
			kinga.Should().Be.EqualTo(ver1);
			kinga.Addresses.Should().Have.SameValuesAs(ver1.Addresses);
			kinga.OwnedAddresses.Should().Have.SameValuesAs(ver1.OwnedAddresses);

			var cracow = new Address {Id = cracowId, City = "Cracow"};
			kinga.OwnedAddresses.Add(cracow);
			kinga.Addresses.Add(cracow);

			var ver2 = AuditReader().Find<Person>(kingaId, 2);
			kinga.Should().Be.EqualTo(ver2);
			kinga.Addresses.Should().Have.SameValuesAs(ver2.Addresses);
			kinga.OwnedAddresses.Should().Have.SameValuesAs(ver2.OwnedAddresses);

			kinga.OwnedAddresses.Add(warsaw);
			cracow.City = "Krakow";

			var ver5 = AuditReader().Find<Person>(kingaId, 5);
			kinga.Addresses.Should().Have.SameValuesAs(ver5.Addresses);
			kinga.OwnedAddresses.Should().Have.SameValuesAs(ver5.OwnedAddresses);
		}

		[Test]
		public void VerifyHistoryOfCracow()
		{
			var cracow = new Address { Id = cracowId, City = "Cracow" };
			var kinga = new Person { Id = kingaId, Name = "Kinga" };
			cracow.Tenants.Add(kinga);
			cracow.Landlord = kinga;

			var ver2 = AuditReader().Find<Address>(cracowId, 2);
			ver2.Should().Be.EqualTo(cracow);
			ver2.Tenants.Should().Have.SameValuesAs(cracow.Tenants);
			ver2.Landlord.Id.Should().Be.EqualTo(cracow.Landlord.Id);

			cracow.City = "Krakow";
			var ver3 = AuditReader().Find<Address>(cracowId, 3);
			ver3.Should().Be.EqualTo(cracow);
		}

		[Test]
		public void VerifyHistoryOfWarsaw()
		{
			var warsaw = new Address { Id = warsawId, City = "Warsaw" };
			var kinga = new Person { Id = kingaId, Name = "Kinga" };
			var lukasz = new Person { Id = lukaszId, Name = "Lukasz" };
			warsaw.Tenants.Add(kinga);
			warsaw.Tenants.Add(lukasz);
			warsaw.Landlord = lukasz;

			var ver1 = AuditReader().Find<Address>(warsawId, 1);
			ver1.Should().Be.EqualTo(warsaw);
			ver1.Tenants.Should().Have.SameValuesAs(warsaw.Tenants);
			ver1.Landlord.Id.Should().Be.EqualTo(warsaw.Landlord.Id);

			warsaw.Landlord = kinga;

			var ver5 = AuditReader().Find<Address>(warsawId, 5);
			ver5.Landlord.Id.Should().Be.EqualTo(warsaw.Landlord.Id);
		}
	}
}