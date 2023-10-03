using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Collection;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Collection
{
	public partial class StringSetTest : TestBase
	{
		private int sse1_id;
		private int sse2_id;

		public StringSetTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Collection.Mapping.hbm.xml" }; }
		}

		protected override void Initialize()
		{
			var sse1 = new StringSetEntity();
			var sse2 = new StringSetEntity();
			// Revision 1 (sse1: initialy empty, sse2: initialy 2 elements)
			using(var tx = Session.BeginTransaction())
			{
				sse2.Strings.Add("sse2_string1");
				sse2.Strings.Add("sse2_string2");
				sse1_id = (int) Session.Save(sse1);
				sse2_id = (int) Session.Save(sse2);
				tx.Commit();
			}
			// Revision 2 (sse1: adding 2 elements, sse2: adding an existing element)
			using (var tx = Session.BeginTransaction())
			{
				sse1.Strings.Add("sse1_string1");
				sse1.Strings.Add("sse1_string2");
				sse2.Strings.Add("sse2_string1");
				tx.Commit();
			}
			// Revision 3 (sse1: removing a non-existing element, sse2: removing one element)
			using (var tx = Session.BeginTransaction())
			{
				sse1.Strings.Remove("sse1_string3");
				sse2.Strings.Remove("sse2_string1");
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(StringSetEntity), sse1_id));
			CollectionAssert.AreEquivalent(new[] { 1, 3 }, AuditReader().GetRevisions(typeof(StringSetEntity), sse2_id));
		}

		[Test]
		public void VerifyHistoryOf1()
		{
			var rev1 = AuditReader().Find<StringSetEntity>(sse1_id, 1);
			var rev2 = AuditReader().Find<StringSetEntity>(sse1_id, 2);
			var rev3 = AuditReader().Find<StringSetEntity>(sse1_id, 3);

			CollectionAssert.IsEmpty(rev1.Strings);
			CollectionAssert.AreEquivalent(new[] { "sse1_string1", "sse1_string2" }, rev2.Strings);
			CollectionAssert.AreEquivalent(new[] { "sse1_string1", "sse1_string2" }, rev3.Strings);
		}

		[Test]
		public void VerifyHistoryOf2()
		{
			var rev1 = AuditReader().Find<StringSetEntity>(sse2_id, 1);
			var rev2 = AuditReader().Find<StringSetEntity>(sse2_id, 2);
			var rev3 = AuditReader().Find<StringSetEntity>(sse2_id, 3);

			CollectionAssert.AreEquivalent(new[] { "sse2_string1", "sse2_string2" }, rev1.Strings);
			CollectionAssert.AreEquivalent(new[] { "sse2_string1", "sse2_string2" }, rev2.Strings);
			CollectionAssert.AreEquivalent(new[] { "sse2_string2" }, rev3.Strings);
		}
	}
}