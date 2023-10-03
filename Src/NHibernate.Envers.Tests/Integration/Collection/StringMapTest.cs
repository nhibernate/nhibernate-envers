using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Tests.Entities.Collection;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Collection
{
	public partial class StringMapTest : TestBase
	{
		private int sme1_id;
		private int sme2_id;

		public StringMapTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Collection.Mapping.hbm.xml" }; }
		}

		protected override void Initialize()
		{
			var sme1 = new StringMapEntity();
			var sme2 = new StringMapEntity();
			// Revision 1 (sme1: initialy empty, sme2: initialy 1 mapping)
			using(var tx = Session.BeginTransaction())
			{
				sme2.Strings["1"] = "a";
				sme1_id = (int)Session.Save(sme1);
				sme2_id = (int)Session.Save(sme2);
				tx.Commit();
			}
			// Revision 2 (sme1: adding 2 mappings, sme2: no changes)
			using(var tx = Session.BeginTransaction())
			{
				sme1.Strings["1"] = "a";
				sme1.Strings["2"] = "b";
				tx.Commit();
			}
			// Revision 3 (sme1: removing an existing mapping, sme2: replacing a value)
			using(var tx = Session.BeginTransaction())
			{
				sme1.Strings.Remove("1");
				sme2.Strings["1"] = "b";
				tx.Commit();
			}
			// No revision (sme1: removing a non-existing mapping, sme2: replacing with the same value)
			using(var tx = Session.BeginTransaction())
			{
				sme1.Strings.Remove("3");
				sme2.Strings["1"] = "b";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, AuditReader().GetRevisions(typeof(StringMapEntity), sme1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 3 }, AuditReader().GetRevisions(typeof(StringMapEntity), sme2_id));
		}

		[Test]
		public void VerifyHistoryOf1()
		{
			var rev1 = AuditReader().Find<StringMapEntity>(sme1_id, 1);
			var rev2 = AuditReader().Find<StringMapEntity>(sme1_id, 2);
			var rev3 = AuditReader().Find<StringMapEntity>(sme1_id, 3);
			var rev4 = AuditReader().Find<StringMapEntity>(sme1_id, 4);

			CollectionAssert.IsEmpty(rev1.Strings.Keys);
			Assert.AreEqual(new Dictionary<string, string> { { "1", "a" }, { "2", "b" } }, rev2.Strings.OrderBy(kv => kv.Key));
			Assert.AreEqual(new Dictionary<string, string> { { "2", "b" } }, rev3.Strings);
			Assert.AreEqual(new Dictionary<string, string> { { "2", "b" } }, rev4.Strings);
		}

		[Test]
		public void VerifyHistoryOf2()
		{
			var rev1 = AuditReader().Find<StringMapEntity>(sme2_id, 1);
			var rev2 = AuditReader().Find<StringMapEntity>(sme2_id, 2);
			var rev3 = AuditReader().Find<StringMapEntity>(sme2_id, 3);
			var rev4 = AuditReader().Find<StringMapEntity>(sme2_id, 4);

			Assert.AreEqual(new Dictionary<string, string> { { "1", "a" } }, rev1.Strings);
			Assert.AreEqual(new Dictionary<string, string> { { "1", "a" } }, rev2.Strings);
			Assert.AreEqual(new Dictionary<string, string> { { "1", "b" } }, rev3.Strings);
			Assert.AreEqual(new Dictionary<string, string> { { "1", "b" } }, rev4.Strings);
		}
	}
}