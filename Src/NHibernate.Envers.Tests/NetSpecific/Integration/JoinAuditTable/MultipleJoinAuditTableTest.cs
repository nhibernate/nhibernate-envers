using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.JoinAuditTable
{
	[TestFixture]
	public class MultipleJoinAuditTableTest : TestBase
	{
		private const int id = 425;

		protected override void Initialize()
		{
			var animal = new Animal{Id=id};
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(animal);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				animal.Height = 14;
				animal.Weight = 45;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions<Animal>(id));
		}

		[Test]
		public void VerifyHistoryOfOwning()
		{
			var ver1 = new Animal(){ Id = id};
			var ver2 = new Animal(){ Id = id, Height = 14, Weight = 45};

			AuditReader().Find<Animal>(id, 1)
				.Should().Be.EqualTo(ver1);
			AuditReader().Find<Animal>(id, 2)
				.Should().Be.EqualTo(ver2);
		}

		[Test]
		public void VerifyTableNames()
		{
			const string auditName = TestAssembly + ".NetSpecific.Integration.JoinAuditTable.Animal_AUD";
			var joinAuditTables = Cfg.GetClassMapping(auditName).JoinIterator;

			joinAuditTables.Count().Should().Be.EqualTo(2);
			joinAuditTables.Where(table => table.Table.Name.Equals("HeightTableAuditing"))
				.Count().Should().Be.EqualTo(1);
			joinAuditTables.Where(table => table.Table.Name.Equals("WeightTableAuditing"))
				.Count().Should().Be.EqualTo(1);
		}

	}
}