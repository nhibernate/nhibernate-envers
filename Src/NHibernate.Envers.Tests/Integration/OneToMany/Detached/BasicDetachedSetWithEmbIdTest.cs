using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Ids;
using NHibernate.Envers.Tests.Entities.OneToMany.Detached.Ids;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.OneToMany.Detached
{
	public partial class BasicDetachedSetWithEmbIdTest : TestBase
	{
		private EmbId str1_id;
		private EmbId str2_id;
		private EmbId coll1_id;

		public BasicDetachedSetWithEmbIdTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			str1_id = new EmbId {X = 1, Y = 2};
			str2_id = new EmbId { X = 3, Y = 4 };
			coll1_id = new EmbId{X = 5, Y = 6};
			var str1 = new EmbIdTestEntity {Id = str1_id, Str1 = "str1"};
			var str2 = new EmbIdTestEntity { Id = str2_id, Str1 = "str2" };
			var coll1 = new SetRefCollEntityEmbId {Id = coll1_id, Data = "coll1"};

			using (var tx = Session.BeginTransaction())
			{
				Session.Save(str1);
				Session.Save(str2);
				coll1.Collection = new HashSet<EmbIdTestEntity> {str1};
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
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4 }, AuditReader().GetRevisions(typeof(SetRefCollEntityEmbId), coll1_id));

			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(EmbIdTestEntity), str1_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(EmbIdTestEntity), str2_id));
		}

		[Test]
		public void VerifyHistoryOfColl1()
		{
			var str1 = Session.Get<EmbIdTestEntity>(str1_id);
			var str2 = Session.Get<EmbIdTestEntity>(str2_id);

			var rev1 = AuditReader().Find<SetRefCollEntityEmbId>(coll1_id, 1);
			var rev2 = AuditReader().Find<SetRefCollEntityEmbId>(coll1_id, 2);
			var rev3 = AuditReader().Find<SetRefCollEntityEmbId>(coll1_id, 3);
			var rev4 = AuditReader().Find<SetRefCollEntityEmbId>(coll1_id, 4);

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
				return new[] { "Entities.Ids.Mapping.hbm.xml", "Entities.OneToMany.Detached.Ids.Mapping.hbm.xml" };
			}
		}
	}
}