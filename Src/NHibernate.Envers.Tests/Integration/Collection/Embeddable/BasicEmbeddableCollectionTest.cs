using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Collection.Embeddable
{
	public partial class BasicEmbeddableCollectionTest : TestBase
	{
		private const int id = 11;

		public BasicEmbeddableCollectionTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var darkCharacter = new DarkCharacter {Id = id, Kills = 1};

			// Revision 1 - empty element collection
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(darkCharacter);
				tx.Commit();
			}

			// Revision 2 - adding collection element
			using (var tx = Session.BeginTransaction())
			{
				darkCharacter.Names.Add(new Name {FirstName = "Action", LastName = "Hank"});
				tx.Commit();
			}

			// Revision 3 - adding another collection element
			using (var tx = Session.BeginTransaction())
			{
				darkCharacter.Names.Add(new Name { FirstName = "Green", LastName = "Lantern" });
				tx.Commit();
			}

			// Revision 4 - removing single collection element
			using (var tx = Session.BeginTransaction())
			{
				darkCharacter.Names.Remove(new Name { FirstName = "Action", LastName = "Hank" });
				tx.Commit();
			}

			// Revision 5 - removing all collection elements
			using (var tx = Session.BeginTransaction())
			{
				darkCharacter.Names.Clear();
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			AuditReader().GetRevisions(typeof (DarkCharacter), id)
			             .Should().Have.SameSequenceAs(1, 2, 3, 4, 5);
		}

		[Test]
		public void VerifyHistoryOfCharacter()
		{
			var darkCharacter = new DarkCharacter {Id = id, Kills = 1};
			var ver1 = AuditReader().Find<DarkCharacter>(id, 1);
			ver1.Should().Be.EqualTo(darkCharacter);
			ver1.Names.Should().Be.Empty();

			darkCharacter.Names.Add(new Name { FirstName = "Action", LastName = "Hank" });
			var ver2 = AuditReader().Find<DarkCharacter>(id, 2);
			ver2.Should().Be.EqualTo(darkCharacter);
			ver2.Names.Should().Have.SameValuesAs(darkCharacter.Names);

			darkCharacter.Names.Add(new Name { FirstName = "Green", LastName = "Lantern" });
			var ver3 = AuditReader().Find<DarkCharacter>(id, 3);
			ver3.Should().Be.EqualTo(darkCharacter);
			ver3.Names.Should().Have.SameValuesAs(darkCharacter.Names);

			darkCharacter.Names.Remove(new Name { FirstName = "Action", LastName = "Hank" });
			var ver4 = AuditReader().Find<DarkCharacter>(id, 4);
			ver4.Should().Be.EqualTo(darkCharacter);
			ver4.Names.Should().Have.SameValuesAs(darkCharacter.Names);

			darkCharacter.Names.Clear();
			var ver5 = AuditReader().Find<DarkCharacter>(id, 5);
			ver5.Should().Be.EqualTo(darkCharacter);
			ver5.Names.Should().Have.SameValuesAs(darkCharacter.Names);
		}

		protected override System.Collections.Generic.IEnumerable<string> Mappings
		{
			get { return new[] {"Integration.Collection.Embeddable.DarkCharacter.hbm.xml"}; }
		}
	}
}