using NHibernate.Envers.Query;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Query
{
	public partial class ComponentQueryTest : TestBase
	{
		private const int id = 1345;
		private const int personWeight = 80;

		public ComponentQueryTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var person = new Person { Id = id, Weight = new Weight { Kilo = personWeight } };
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(person);
				tx.Commit();
			}
		}

		[Test]
		public void CanQueryComponentProperty_UsingDot()
		{
			Session.Auditer().CreateQuery()
				.ForRevisionsOf<Person>()
				.Add(AuditEntity.Property("Weight.Kilo").Eq(personWeight))
				.Results()
				.Should().Have.Count.EqualTo(1);
		}

		[Test]
		public void CanQueryComponentProperty_UsingUnderscore()
		{
			Session.Auditer().CreateQuery()
				.ForRevisionsOf<Person>()
				.Add(AuditEntity.Property("Weight_Kilo").Eq(personWeight))
				.Results()
				.Should().Have.Count.EqualTo(1);
		}
	}
}