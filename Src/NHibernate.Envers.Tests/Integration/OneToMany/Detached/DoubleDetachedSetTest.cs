using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.OneToMany.Detached;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.OneToMany.Detached
{
	public partial class DoubleDetachedSetTest : TestBase
	{
		private int str1_id;
		private int str2_id;
		private int coll1_id;

		public DoubleDetachedSetTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			coll1_id = 333;
			var str1 = new StrTestEntity {Str = "str1"};
			var str2 = new StrTestEntity {Str = "str2"};
			var coll1 = new DoubleSetRefCollEntity {Id = coll1_id, Data = "coll1"};

			using (var tx = Session.BeginTransaction())
			{
				str1_id = (int) Session.Save(str1);
				str2_id = (int) Session.Save(str2);
				coll1.Collection = new HashSet<StrTestEntity> {str1};
				Session.Save(coll1);
				coll1.Collection2 = new HashSet<StrTestEntity> {str2};
				Session.Save(coll1);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				coll1.Collection.Add(str2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				coll1.Collection.Remove(str1);
				coll1.Collection2.Add(str1);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, AuditReader().GetRevisions(typeof(DoubleSetRefCollEntity), coll1_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(StrTestEntity), str1_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(StrTestEntity), str2_id));
		}


		[Test]
		public void VerifyHistoryOfColl1()
		{
			var str1 = Session.Get<StrTestEntity>(str1_id);
			var str2 = Session.Get<StrTestEntity>(str2_id);

			var rev1 = AuditReader().Find<DoubleSetRefCollEntity>(coll1_id, 1);
			var rev2 = AuditReader().Find<DoubleSetRefCollEntity>(coll1_id, 2);
			var rev3 = AuditReader().Find<DoubleSetRefCollEntity>(coll1_id, 3);

			CollectionAssert.AreEquivalent(new[] { str1 }, rev1.Collection);
			CollectionAssert.AreEquivalent(new[] { str1, str2 }, rev2.Collection);
			CollectionAssert.AreEquivalent(new[] { str2 }, rev3.Collection);

			CollectionAssert.AreEquivalent(new[] { str2 }, rev1.Collection2);
			CollectionAssert.AreEquivalent(new[] { str2 }, rev2.Collection2);
			CollectionAssert.AreEquivalent(new[] { str1, str2 }, rev3.Collection2);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml", "Entities.OneToMany.Detached.Mapping.hbm.xml" };
			}
		}
	}
}