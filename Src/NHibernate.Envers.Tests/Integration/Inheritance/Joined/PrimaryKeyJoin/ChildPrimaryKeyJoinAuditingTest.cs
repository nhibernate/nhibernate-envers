using System.Linq;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Tests.Integration.Inheritance.Entities;
using NHibernate.Mapping;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Joined.PrimaryKeyJoin
{
	[Audited]
	public partial class ChildPrimaryKeyJoinAuditingTest : TestBase
	{
		private int id1;

		public ChildPrimaryKeyJoinAuditingTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			id1 = 1;
			var ce = new ChildPrimaryKeyJoinEntity {Id = id1, Data = "x", Number = 1};
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ce);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ce.Data = "y";
				ce.Number = 2;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] {1, 2}, AuditReader().GetRevisions(typeof(ChildPrimaryKeyJoinEntity),id1));
		}

		[Test]
		public void VerifyHistoryOfChild()
		{
			var ver1 = new ChildPrimaryKeyJoinEntity {Id = id1, Data = "x", Number = 1};
			var ver2 = new ChildPrimaryKeyJoinEntity {Id = id1, Data = "y", Number = 2};

			Assert.AreEqual(ver1, AuditReader().Find<ChildPrimaryKeyJoinEntity>(id1, 1));
			Assert.AreEqual(ver1, AuditReader().Find<ParentEntity>(id1, 1));
			Assert.AreEqual(ver2, AuditReader().Find<ChildPrimaryKeyJoinEntity>(id1, 2));
			Assert.AreEqual(ver2, AuditReader().Find<ParentEntity>(id1, 2));
		}

		[Test]
		public void VerifyPolymorphicQuery()
		{
			var ver1 = new ChildPrimaryKeyJoinEntity {Id = id1, Data = "x", Number = 1};
			Assert.AreEqual(ver1,
			                AuditReader().CreateQuery().ForEntitiesAtRevision(typeof (ChildPrimaryKeyJoinEntity), 1).
			                	GetSingleResult());
			Assert.AreEqual(ver1, AuditReader().CreateQuery().ForEntitiesAtRevision(typeof (ParentEntity), 1).GetSingleResult());
		}

		[Test]
		public void VerifyPolymorphicQueryGeneric()
		{
			var ver1 = new ChildPrimaryKeyJoinEntity { Id = id1, Data = "x", Number = 1 };
			AuditReader().CreateQuery().ForEntitiesAtRevision<ChildPrimaryKeyJoinEntity>(1).Single().Should().Be(ver1);
			AuditReader().CreateQuery().ForEntitiesAtRevision<ParentEntity>(1).Single().Should().Be(ver1);
		}

		[Test]
		public void VerifyChildIdColumnName()
		{
			var auditName = TestAssembly + ".Integration.Inheritance.Entities.ChildPrimaryKeyJoinEntity_AUD";
			var keyColumn = (Column)Cfg.GetClassMapping(auditName).Key.ColumnIterator.First();
			Assert.AreEqual("OtherId", keyColumn.Name);
		}
	}
} 