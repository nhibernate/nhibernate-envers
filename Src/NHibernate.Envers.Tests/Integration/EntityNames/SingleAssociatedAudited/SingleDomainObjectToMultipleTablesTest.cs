using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.EntityNames.SingleAssociatedAudited
{
	public partial class SingleDomainObjectToMultipleTablesTest : TestBase
	{
		private long carId;
		private long ownerId;
		private long driverId;

		public SingleDomainObjectToMultipleTablesTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			using (var tx = Session.BeginTransaction())
			{
				var owner = new Person {Name = "Lukasz", Age = 25};
				var driver = new Person {Name = "Kinga", Age = 24};
				var car = new Car {Number = 1, Driver = driver, Owner = owner};
				ownerId = (long) Session.Save("Personaje", owner);
				driverId = (long) Session.Save("Driveraje", driver);
				carId = (long) Session.Save(car);
				tx.Commit();
			}
		}

		[Test]
		public void VerifySingleDomainObjectToMultipleTablesMapping()
		{
			var carVer1 = AuditReader().Find<Car>(carId, 1);
			var ownerVer1 = (Person)AuditReader().Find("Personaje", ownerId, 1);
			var driverVer1 = (Person)AuditReader().Find("Driveraje", driverId, 1);

			carVer1.Owner.Id.Should().Be.EqualTo(ownerVer1.Id);
			carVer1.Driver.Id.Should().Be.EqualTo(driverVer1.Id);

			ownerVer1.Name.Should().Be.EqualTo("Lukasz");
			driverVer1.Name.Should().Be.EqualTo("Kinga");
			carVer1.Number.Should().Be.EqualTo(1);
		}
	}
}