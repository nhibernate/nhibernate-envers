using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Integration.EntityNames.ManyToManyAudited;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public partial class HasChangedAuditedManyToManyTest : AbstractModifiedFlagsEntityTest
	{
		private long idCar1;
		private long idPers1;
		private long idPers2;

		public HasChangedAuditedManyToManyTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var pers1 = new Person {Name = "Hernan", Age = 28};
			var pers2 = new Person {Name = "Leandro", Age = 29 };
			var pers3 = new Person {Name = "Barba", Age = 32};
			var pers4 = new Person {Name = "Camomo", Age = 15};

			var owners1 = new List<Person> { pers1, pers2, pers3 };
			var car1 = new Car { Number = 5, Owners = owners1 };
			
			//rev 1
			using (var tx = Session.BeginTransaction())
			{
				idCar1 = (long) Session.Save(car1);
				tx.Commit();
			}
			idPers1 = pers1.Id;
			idPers2 = pers2.Id;

			var owners2 = new List<Person> { pers2, pers3, pers4 };
			var car2 = new Car { Number = 27, Owners = owners2 };
			//rev 2
			using (var tx = Session.BeginTransaction())
			{

				pers1.Name = "Hernan David";
				pers1.Age = 40;
				Session.Save(car1);
				Session.Save(car2);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyHasChangedPerson1()
		{
			AuditReader().CreateQuery().ForRevisionsOfEntity("Personaje", false, false)
						.Add(AuditEntity.Id().Eq(idPers1))
						.Add(AuditEntity.Property("Cars").HasChanged())
						.GetResultList()
						.ExtractRevisionNumbersFromRevision()
						.Should().Have.SameSequenceAs(1);

			AuditReader().CreateQuery().ForRevisionsOfEntity("Personaje", false, false)
						.Add(AuditEntity.Id().Eq(idPers1))
						.Add(AuditEntity.Property("Cars").HasNotChanged())
						.GetResultList()
						.ExtractRevisionNumbersFromRevision()
						.Should().Have.SameSequenceAs(2);
		}

		[Test]
		public void VerifyHasChangedPerson2()
		{
			AuditReader().CreateQuery().ForRevisionsOfEntity("Personaje", false, false)
						.Add(AuditEntity.Id().Eq(idPers2))
						.Add(AuditEntity.Property("Cars").HasChanged())
						.GetResultList()
						.ExtractRevisionNumbersFromRevision()
						.Should().Have.SameSequenceAs(1, 2);

			AuditReader().CreateQuery().ForRevisionsOfEntity("Personaje", false, false)
						.Add(AuditEntity.Id().Eq(idPers2))
						.Add(AuditEntity.Property("Cars").HasNotChanged())
						.GetResultList()
						.ExtractRevisionNumbersFromRevision()
						.Should().Be.Empty();
		}

		[Test]
		public void VerifyHasChangedCar1()
		{
			var list = AuditReader().CreateQuery().ForHistoryOf<Car, DefaultRevisionEntity>(false)
						.Add(AuditEntity.Id().Eq(idCar1))
						.Add(AuditEntity.Property("Owners").HasChanged())
						.Results().ToList();
			list.Should().Have.Count.EqualTo(1);
			list.ExtractRevisionNumbersFromHistory().Should().Have.SameSequenceAs(1);

			var list2 = AuditReader().CreateQuery().ForRevisionsOfEntity(typeof(Car), false, false)
						.Add(AuditEntity.Id().Eq(idCar1))
						.Add(AuditEntity.Property("Owners").HasNotChanged())
						.GetResultList();
			list2.Count.Should().Be.EqualTo(0);
		}


		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Integration.EntityNames.ManyToManyAudited.Mapping.hbm.xml" };
			}
		}
	}
}