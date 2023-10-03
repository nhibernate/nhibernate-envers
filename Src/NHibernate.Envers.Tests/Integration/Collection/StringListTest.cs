using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Collection;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Collection
{
	public partial class StringListTest : TestBase
	{
		private int sle1_id;
		private int sle2_id;

		public StringListTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Collection.Mapping.hbm.xml" }; }
		}

		protected override void Initialize()
		{
			var sle1 = new StringListEntity();
			var sle2 = new StringListEntity();

			// Revision 1 (sle1: initialy empty, sle2: initialy 2 elements)
			using(var tx = Session.BeginTransaction())
			{
				sle2.Strings.Add("sle2_string1");
				sle2.Strings.Add("sle2_string2");
				sle1_id = (int)Session.Save(sle1);
				sle2_id = (int)Session.Save(sle2);
				tx.Commit();
			}

			// Revision 2 (sle1: adding 2 elements, sle2: adding an existing element)
			using(var tx = Session.BeginTransaction())
			{
				sle1.Strings.Add("sle1_string1");
				sle1.Strings.Add("sle1_string2");
				sle2.Strings.Add("sle2_string1");
				tx.Commit();
			}

			// Revision 3 (sle1: replacing an element at index 0, sle2: removing an element at index 0)
			using(var tx = Session.BeginTransaction())
			{
				sle1.Strings[0] = "sle1_string3";
				sle2.Strings.RemoveAt(0);
				tx.Commit();
			}
		}


		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, AuditReader().GetRevisions(typeof(StringListEntity), sle1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, AuditReader().GetRevisions(typeof(StringListEntity), sle2_id));
		}

		[Test]
		public void VerifyHistoryOf1()
		{
			var rev1 = AuditReader().Find<StringListEntity>(sle1_id, 1);
			var rev2 = AuditReader().Find<StringListEntity>(sle1_id, 2);
			var rev3 = AuditReader().Find<StringListEntity>(sle1_id, 3);

			CollectionAssert.IsEmpty(rev1.Strings);
			CollectionAssert.AreEqual(new[] { "sle1_string1", "sle1_string2" }, rev2.Strings);
			CollectionAssert.AreEqual(new[] { "sle1_string3", "sle1_string2" }, rev3.Strings);
		}

		[Test]
		public void VerifyHistoryOf2()
		{
			var rev1 = AuditReader().Find<StringListEntity>(sle2_id, 1);
			var rev2 = AuditReader().Find<StringListEntity>(sle2_id, 2);
			var rev3 = AuditReader().Find<StringListEntity>(sle2_id, 3);

			CollectionAssert.AreEqual(new[] { "sle2_string1", "sle2_string2" }, rev1.Strings);
			CollectionAssert.AreEqual(new[] { "sle2_string1", "sle2_string2", "sle2_string1" }, rev2.Strings);
			CollectionAssert.AreEqual(new[] { "sle2_string2", "sle2_string1" }, rev3.Strings);
		}
	}
}