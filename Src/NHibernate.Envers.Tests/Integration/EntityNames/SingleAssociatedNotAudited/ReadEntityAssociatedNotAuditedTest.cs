﻿using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.EntityNames.SingleAssociatedNotAudited
{
	[TestFixture]
	public class ReadEntityAssociatedNotAuditedTest : TestBase
	{
		private long id_car1;
		private long id_car2;

		private long id_pers1;
		private long id_pers2;

		private Car car1;
		private Car car2;
		private Person person1_1;
		private Person person2;
		private Person currentPerson1;
		private Car currentCar1;

		protected override void Initialize()
		{
			var pers1 = new Person { Name = "Hernan", Age = 15 };
			var pers2 = new Person { Name = "Leandro", Age = 19 };

			var car1 = new Car { Number = 1, Owner = pers1 };
			var car2 = new Car { Number = 2, Owner = pers2 };

			//rev 1
			using (var tx = Session.BeginTransaction())
			{
				id_pers1 = (long)Session.Save("Personaje", pers1);
				id_car1 = (long)Session.Save(car1);
				tx.Commit();
			}

			//rev 2
			using (var tx = Session.BeginTransaction())
			{
				pers1.Age = 50;
				id_pers2 = (long)Session.Save("Personaje", pers2);
				id_car2 = (long)Session.Save(car2);
				tx.Commit();
			}
		}

		private void loadDataOnSessionAndAuditReader()
		{
			currentPerson1 = (Person)Session.Get("Personaje", id_pers1);
			person2 = (Person) Session.Get("Personaje", id_pers2);

			currentCar1 = Session.Get<Car>(id_car1);

			car1 = AuditReader().Find<Car>(id_car1, 1);
			car2 = AuditReader().Find<Car>(id_car2, 2);
		}

		private void checkEntityNames()
		{
			AuditReader().GetEntityName(id_car1, 1, car1)
				.Should().Be.EqualTo(Session.GetEntityName(currentCar1));
		}

		private void checkEntities()
		{
			person1_1 = car1.Owner;
			var person2_1 = car2.Owner;

			currentPerson1.Age.Should().Be.EqualTo(person1_1.Age);
			person2.Age.Should().Be.EqualTo(person2_1.Age);
		}

		[Test]
		public void ShouldObtainEntityNameAssociationWithEntityNameAndNotAuditedMode()
		{
			loadDataOnSessionAndAuditReader();
			checkEntities();
			checkEntityNames();
		}

		[Test]
		public void ShouldtestObtainEntityNameAssociationWithEntityNameAndNotAuditedModeInNewSession()
		{
			ForceNewSession();
			loadDataOnSessionAndAuditReader();
			checkEntities();
			checkEntityNames();
		}
	}
}