using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.OneToMany.Detached;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.OneToMany.Detached
{
	public partial class BasicDetachedSetTest : TestBase
	{
		private int str1_id;
		private int str2_id;
		private const int coll1_id = 13;

		public BasicDetachedSetTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var str1 = new StrTestEntity { Str = "str1" };
			var str2 = new StrTestEntity { Str = "str2" };
			var coll1 = new SetRefCollEntity { Id = coll1_id, Data = "coll1" };

			using (var tx = Session.BeginTransaction())
			{
				str1_id = (int)Session.Save(str1);
				str2_id = (int)Session.Save(str2);
				coll1.Collection = new HashSet<StrTestEntity> { str1 };
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
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				coll1.Collection.Clear();
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4 }, AuditReader().GetRevisions(typeof(SetRefCollEntity), coll1_id));

			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(StrTestEntity), str1_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(StrTestEntity), str2_id));
		}


		[Test]
		public void VerifyHistoryOfColl1()
		{
			var str1 = Session.Get<StrTestEntity>(str1_id);
			var str2 = Session.Get<StrTestEntity>(str2_id);

			var rev1 = AuditReader().Find<SetRefCollEntity>(coll1_id, 1);
			var rev2 = AuditReader().Find<SetRefCollEntity>(coll1_id, 2);
			var rev3 = AuditReader().Find<SetRefCollEntity>(coll1_id, 3);
			var rev4 = AuditReader().Find<SetRefCollEntity>(coll1_id, 4);

			CollectionAssert.AreEquivalent(new[] { str1 }, rev1.Collection);
			CollectionAssert.AreEquivalent(new[] { str1, str2 }, rev2.Collection);
			CollectionAssert.AreEquivalent(new[] { str2 }, rev3.Collection);
			CollectionAssert.IsEmpty(rev4.Collection);

			Assert.AreEqual("coll1", rev1.Data);
			Assert.AreEqual("coll1", rev2.Data);
			Assert.AreEqual("coll1", rev3.Data);
			Assert.AreEqual("coll1", rev4.Data);
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