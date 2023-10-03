using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.EntityNames.AuditedEntity
{
	public partial class ReadEntityWithEntityNameTest : TestBase
	{
		private long id_pers1;
		private long id_pers2;
		private long id_pers3;

		public ReadEntityWithEntityNameTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var pers1 = new Person {Name = "Hernan", Age = 28};
			var pers2 = new Person {Name = "Leandro", Age = 29};
			var pers3 = new Person {Name = "Barba", Age = 30};

			//rev1
			using (var tx = Session.BeginTransaction())
			{
				id_pers1 = (long) Session.Save("Personaje", pers1);
				tx.Commit();
			}

			//rev2
			using (var tx = Session.BeginTransaction())
			{
				pers1.Age = 29;
				id_pers2 = (long)Session.Save("Personaje", pers2);
				tx.Commit();
			}

			//rev3
			using (var tx = Session.BeginTransaction())
			{
				pers1.Name = "Hernan David";
				pers2.Age = 30;
				id_pers3 = (long)Session.Save("Personaje", pers3);
				tx.Commit();
			}
		}

		[Test]
		public void ShouldRetrieveRevisionsWithEntityName()
		{
			var pers1Revs = AuditReader().GetRevisions("Personaje", id_pers1);
			var pers2Revs = AuditReader().GetRevisions("Personaje", id_pers2);
			var pers3Revs = AuditReader().GetRevisions("Personaje", id_pers3);

			pers1Revs.Should().Have.Count.EqualTo(3);
			pers2Revs.Should().Have.Count.EqualTo(2);
			pers3Revs.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void ShouldRetrieveAuditedEntityWithEntityName()
		{
			AuditReader().Find("Personaje", id_pers1, 1).Should().Not.Be.Null();
			AuditReader().Find("Personaje", id_pers1, 2).Should().Not.Be.Null();
			AuditReader().Find("Personaje", id_pers1, 3).Should().Not.Be.Null();
		}

		[Test]
		public void ShouldObtainEntityNameAuditedEntityWithEntityName()
		{
			var person1_1 = AuditReader().Find("Personaje", id_pers1, 1);
			var person1_2 = AuditReader().Find("Personaje", id_pers1, 2);
			var person1_3 = AuditReader().Find("Personaje", id_pers1, 3);

			var person1EN = AuditReader().GetEntityName(id_pers1, 1, person1_1);
			var person2EN = AuditReader().GetEntityName(id_pers1, 2, person1_2);
			var person3EN = AuditReader().GetEntityName(id_pers1, 3, person1_3);

			person1EN.Should().Be.EqualTo("Personaje");
			person2EN.Should().Be.EqualTo("Personaje");
			person3EN.Should().Be.EqualTo("Personaje");
		}

		[Test]
		public void ShouldRetrieveAuditedEntityWithEntityNameWithNewSession()
		{
			ForceNewSession();

			AuditReader().Find("Personaje", id_pers1, 1).Should().Not.Be.Null();
			AuditReader().Find("Personaje", id_pers1, 2).Should().Not.Be.Null();
			AuditReader().Find("Personaje", id_pers1, 3).Should().Not.Be.Null();
		}
	}
}