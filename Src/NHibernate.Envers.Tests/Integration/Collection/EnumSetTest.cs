using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Collection;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Collection
{
	public partial class EnumSetTest : TestBase
	{
		private int sse1_id;

		public EnumSetTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Collection.Mapping.hbm.xml" }; }
		}

		protected override void Initialize()
		{
			var sse1 = new EnumSetEntity();
			using(var tx = Session.BeginTransaction())
			{
				sse1.Enums1.Add(E1.X);
				sse1.Enums2.Add(E2.A);
				sse1_id = (int) Session.Save(sse1);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				sse1.Enums1.Add(E1.Y);
				sse1.Enums2.Remove(E2.B);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				sse1.Enums1.Remove(E1.X);
				sse1.Enums2.Add(E2.A);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, AuditReader().GetRevisions(typeof(EnumSetEntity), sse1_id));
		}

		[Test]
		public void VerifyHistoryOf()
		{
			var rev1 = AuditReader().Find<EnumSetEntity>(sse1_id, 1);
			var rev2 = AuditReader().Find<EnumSetEntity>(sse1_id, 2);
			var rev3 = AuditReader().Find<EnumSetEntity>(sse1_id, 3);

			CollectionAssert.AreEquivalent(new[] { E1.X }, rev1.Enums1);
			CollectionAssert.AreEquivalent(new[] { E1.X, E1.Y }, rev2.Enums1);
			CollectionAssert.AreEquivalent(new[] { E1.Y }, rev3.Enums1);

			CollectionAssert.AreEquivalent(new[] { E2.A }, rev1.Enums2);
			CollectionAssert.AreEquivalent(new[] { E2.A }, rev2.Enums2);
			CollectionAssert.AreEquivalent(new[] { E2.A }, rev3.Enums2);
		}
	}
}