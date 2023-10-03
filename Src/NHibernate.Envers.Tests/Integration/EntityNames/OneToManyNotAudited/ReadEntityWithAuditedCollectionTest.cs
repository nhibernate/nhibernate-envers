using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.EntityNames.OneToManyNotAudited
{
	public partial class ReadEntityWithAuditedCollectionTest : TestBase
	{
		private long id_car1;
		private long id_car2;
		private Car car1_1;

		public ReadEntityWithAuditedCollectionTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var pers1 = new Person { Name = "Hernan", Age = 28 };
			var pers2 = new Person { Name = "Leandro", Age = 29 };
			var pers4 = new Person { Name = "Camomo", Age = 15 };

			//rev 1
			using (var tx = Session.BeginTransaction())
			{
				var owners = new List<Person> { pers1, pers2 };
				var car1 = new Car { Number = 5, Owners = owners };
				id_car1 = (long)Session.Save(car1);
				tx.Commit();
			}

			//rev 2
			using (var tx = Session.BeginTransaction())
			{
				var owners = new List<Person> { pers2, pers4 };
				var car2 = new Car { Number = 27, Owners = owners };
				pers1.Name = "Hernan David";
				pers1.Age = 40;
				id_car2 = (long)Session.Save(car2);
				tx.Commit();
			}
		}

		private void loadDataOnSessionAndAuditReader()
		{
			car1_1 = AuditReader().Find<Car>(id_car1, 2);
			var car2 = AuditReader().Find<Car>(id_car2, 2);

			// navigate through relations to load objects
			foreach (var foo in car1_1.Owners.Select(owner => owner.Name))
			{
			}
			foreach (var foo in car2.Owners.Select(owner => owner.Name))
			{
			}
		}

		private void checkEntityNames()
		{
			AuditReader().GetEntityName(id_car1, 2, car1_1)
				.Should().Be.EqualTo(Session.GetEntityName(Session.Get<Car>(id_car1)));
		}

		[Test]
		public void ShouldObtainEntityNameAuditedCollectionWithEntityName()
		{
			loadDataOnSessionAndAuditReader();
			checkEntityNames();
		}

		[Test]
		public void ShouldObtainEntityNameAuditedCollectionWithEntityNameInNewSession()
		{
			ForceNewSession();
			loadDataOnSessionAndAuditReader();
			checkEntityNames();
		}
	}
}