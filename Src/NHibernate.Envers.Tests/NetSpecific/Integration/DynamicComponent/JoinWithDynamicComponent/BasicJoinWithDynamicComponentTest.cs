using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.DynamicComponent.JoinWithDynamicComponent
{
	public partial class BasicJoinWithDynamicComponentTest : TestBase
	{
		private long id_car1;
		private long id_car2;

		private Car currentCar1;
		private Car car1;

		private long id_pers1;
		private long id_pers2;

		private Person currentPerson1;
		private Person person1;

		public BasicJoinWithDynamicComponentTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var pers1 = new Person { Name = "Hernan", Age = 15 };
			var pers2 = new Person { Name = "Leandro", Age = 19 };

			var car1 = new Car { Number = 1, Owner = pers1 };
			var car2 = new Car { Number = 2, Owner = pers2 };

			//rev 1
			using (var tx = Session.BeginTransaction())
			{
				id_pers1 = (long)Session.Save("Person", pers1);
				id_car1 = (long)Session.Save(car1);
				tx.Commit();
			}

			//rev 2
			using (var tx = Session.BeginTransaction())
			{
				pers1.Age = 50;
				id_pers2 = (long)Session.Save("Person", pers2);
				id_car2 = (long)Session.Save(car2);
				tx.Commit();
			}
		}

		private void loadDataOnSessionAndAuditReader()
		{
			currentCar1 = Session.Get<Car>(id_car1);
			currentPerson1 = (Person)Session.Get("Person", id_pers1);
			car1 = AuditReader().Find<Car>(id_car1, 1);
			person1 = car1.Owner;
		}

		private void checkEntities()
		{
			currentPerson1.Age.Should().Not.Be.EqualTo(person1.Age);

			var person2 = (Person)Session.Get("Person", id_pers2);
			var car2 = AuditReader().Find<Car>(id_car2, 2);
			var person2_1 = car2.Owner;
			person2.Age.Should().Be.EqualTo(person2_1.Age);
		}

		private void checkEntityNames()
		{
			AuditReader().GetEntityName(id_car1, 1, car1)
				.Should().Be.EqualTo(Session.GetEntityName(currentCar1));
			AuditReader().GetEntityName(id_pers1, 1, person1)
				.Should().Be.EqualTo("Person");
		}

		[Test]
		public void ShouldGetAssociationWithEntityName()
		{
			loadDataOnSessionAndAuditReader();
			checkEntities();
			checkEntityNames();
		}

		[Test]
		public void ShouldGetAssociationWithEntityNameInNewSession()
		{
			ForceNewSession();
			loadDataOnSessionAndAuditReader();
			checkEntities();
			checkEntityNames();
		}
	}
}