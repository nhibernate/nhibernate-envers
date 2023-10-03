using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.EntityNames.ManyToManyAudited
{
	public partial class ReadEntityWithAuditedManyToManyTest : TestBase
	{
		private long id_car1;
		private long id_car2;
		private long id_pers1;
		private Car car1;
		private Person person1_1;
		private Car car1_2;

		public ReadEntityWithAuditedManyToManyTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var pers1 = new Person { Name = "Hernan", Age = 28 };
			var pers2 = new Person { Name = "Leandro", Age = 29 };
			var pers3 = new Person { Name = "Barba", Age = 32 };
			var pers4 = new Person { Name = "Camomo", Age = 15 };

			//rev 1
			using (var tx = Session.BeginTransaction())
			{
				var owners = new List<Person> { pers1, pers2, pers3 };
				var car1 = new Car { Number = 5, Owners = owners };
				id_car1 = (long)Session.Save(car1);
				tx.Commit();
				id_pers1 = pers1.Id;
			}

			//rev 2
			using (var tx = Session.BeginTransaction())
			{
				var owners = new List<Person> { pers2, pers3, pers4 };
				var car2 = new Car { Number = 27, Owners = owners };
				pers1.Name = "Hernan David";
				pers1.Age = 40;
				id_car2 = (long)Session.Save(car2);
				tx.Commit();
			}
		}

		private void loadDataOnSessionAndAuditReader()
		{
			car1_2 = AuditReader().Find<Car>(id_car1, 2);
			var car2_2 = AuditReader().Find<Car>(id_car2, 2);

			// navigate through relations to load objects
			foreach (var foo in from owner in car1_2.Owners from car in owner.Cars select car.Number)
			{
			}
			foreach (var foo in from owner in car2_2.Owners from car in owner.Cars select car.Number)
			{
			}

			car1 = Session.Get<Car>(id_car1);
			person1_1 = (Person) AuditReader().Find("Personaje", id_pers1, 1);
		}

		private void checkEntityNames()
		{
			AuditReader().GetEntityName(id_pers1, 1, person1_1)
				.Should().Be.EqualTo("Personaje");
			AuditReader().GetEntityName(id_car1, 2, car1_2)
				.Should().Be.EqualTo(Session.GetEntityName(car1));
		}

		[Test]
		public void ShouldGetEntityNameManyToManyWithEntityName()
		{
			loadDataOnSessionAndAuditReader();
			checkEntityNames();
		}

		[Test]
		public void ShouldGetEntityNameManyToManyWithEntityNameInNewSession()
		{
			ForceNewSession();
			loadDataOnSessionAndAuditReader();
			checkEntityNames();
		}
	}
}