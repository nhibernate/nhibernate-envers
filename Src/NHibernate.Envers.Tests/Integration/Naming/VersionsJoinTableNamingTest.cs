using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Naming
{
	public partial class VersionsJoinTableNamingTest : TestBase
	{
		private int uni1_id;
		private int str1_id;

		public VersionsJoinTableNamingTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			uni1_id = 427;
			var uni1 = new VersionsJoinTableTestEntity {Id = uni1_id, Data = "data1", Collection = new HashSet<StrTestEntity>()};
			var str1 = new StrTestEntity {Str = "str1"};
			using(var tx = Session.BeginTransaction())
			{
				Session.Save(uni1);
				str1_id = (int) Session.Save(str1);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				uni1.Collection.Add(str1);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(VersionsJoinTableTestEntity), uni1_id));
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(StrTestEntity), str1_id));
		}

		[Test]
		public void VerifyHistoryOfUniId1()
		{
			var str1 = Session.Get<StrTestEntity>(str1_id);

			var rev1 = AuditReader().Find<VersionsJoinTableTestEntity>(uni1_id, 1);
			var rev2 = AuditReader().Find<VersionsJoinTableTestEntity>(uni1_id, 2);

			CollectionAssert.IsEmpty(rev1.Collection);
			CollectionAssert.AreEquivalent(new[] { str1 }, rev2.Collection);
			Assert.AreEqual("data1", rev1.Data);
			Assert.AreEqual("data1", rev2.Data);
		}

		private const string MIDDLE_VERSIONS_ENTITY_NAME = "VERSIONS_JOIN_TABLE_TEST";

		[Test]
		public void VerifyTableName()
		{
			Assert.AreEqual(MIDDLE_VERSIONS_ENTITY_NAME, Cfg.GetClassMapping(MIDDLE_VERSIONS_ENTITY_NAME).Table.Name);
		}

		[Test]
		public void VerifyJoinColumnName()
		{
			var columns = Cfg.GetClassMapping(MIDDLE_VERSIONS_ENTITY_NAME).Table.ColumnIterator;
			bool id1Found = false;
			bool id2Found = false;

			foreach (var column in columns)
			{
				if (column.Name.Equals("VJT_ID"))
					id1Found = true;
				if (column.Name.Equals("STR_ID"))
					id2Found = true;
			}
			Assert.IsTrue(id1Found && id2Found);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Integration.Naming.Mapping.hbm.xml", "Entities.Mapping.hbm.xml" };
			}
		}
	}
}