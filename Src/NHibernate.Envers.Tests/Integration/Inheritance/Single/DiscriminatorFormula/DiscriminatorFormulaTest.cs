using System.Linq;
using NHibernate.Mapping;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Single.DiscriminatorFormula
{
	public partial class DiscriminatorFormulaTest : TestBase
	{
		private PersistentClass parentAudit;
		private ChildEntity childVer1;
		private ChildEntity childVer2;
		private ParentEntity parentVer1;
		private ParentEntity parentVer2;

		public DiscriminatorFormulaTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			parentAudit = Cfg.GetClassMapping(TestAssembly + ".Integration.Inheritance.Single.DiscriminatorFormula.ParentEntity_AUD");

			long childTypeId;
			long parentTypeId;
			//no rev
			using (var tx = Session.BeginTransaction())
			{
				var childTypeEntity = new ClassTypeEntity {Type = ClassTypeEntity.ChildType};
				var parentTypeEntity = new ClassTypeEntity {Type = ClassTypeEntity.ParentType};
				childTypeId = (long) Session.Save(childTypeEntity);
				parentTypeId = (long) Session.Save(parentTypeEntity);
				tx.Commit();
			}
			var child = new ChildEntity { TypeId = childTypeId, Data = "child data", SpecificData = "child specific data" };
			var parent = new ParentEntity {TypeId = parentTypeId, Data = "parent data"};
			//rev1
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(child);
				tx.Commit();
			}
			//rev2
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(parent);
				tx.Commit();
			}
			//rev 3
			using (var tx = Session.BeginTransaction())
			{
				child.Data = "child data modified";
				tx.Commit();
			}
			//rev 4
			using (var tx = Session.BeginTransaction())
			{
				parent.Data = "parent data modified";
				tx.Commit();
			}
			childVer1 = new ChildEntity {Id = child.Id, TypeId = childTypeId, Data = "child data", SpecificData = "child specific data"};
			childVer2 = new ChildEntity {Id = child.Id, TypeId = childTypeId, Data = "child data modified", SpecificData = "child specific data"};
			parentVer1 = new ParentEntity {Id = parent.Id, TypeId = parentTypeId, Data = "parent data"};
			parentVer2 = new ParentEntity {Id = parent.Id, TypeId = parentTypeId, Data = "parent data modified"};
		}

		[Test]
		public void VerifyDiscriminatorFormulaInAuditTable()
		{
			parentAudit.Discriminator.HasFormula
				.Should().Be.True();
			foreach (var column in parentAudit.Discriminator.ColumnIterator)
			{
				var formula = column as Formula;
				if (formula != null)
				{
					formula.Text
						.Should().Be.EqualTo("(SELECT c.type FROM ClassTypeEntity c WHERE c.id = typeId)");
				}
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 3 }, AuditReader().GetRevisions(typeof(ChildEntity),childVer1.Id));
			CollectionAssert.AreEquivalent(new[] { 2, 4 }, AuditReader().GetRevisions(typeof(ParentEntity),parentVer1.Id));
		}

		[Test]
		public void VerifyHistoryOfParent()
		{
			AuditReader().Find<ParentEntity>(parentVer1.Id, 2)
				.Should().Be.EqualTo(parentVer1);
			AuditReader().Find<ParentEntity>(parentVer2.Id, 4)
				.Should().Be.EqualTo(parentVer2);
		}

		[Test]
		public void VerifyHistoryOfChild()
		{
			AuditReader().Find<ChildEntity>(childVer1.Id, 1)
				.Should().Be.EqualTo(childVer1);
			AuditReader().Find<ChildEntity>(childVer2.Id, 3)
				.Should().Be.EqualTo(childVer2);
		}

		[Test]
		public void PolymorphicQuery()
		{
			AuditReader().CreateQuery().ForEntitiesAtRevision<ChildEntity>(1).Results().First()
				.Should().Be.EqualTo(childVer1);
			AuditReader().CreateQuery().ForEntitiesAtRevision<ParentEntity>(1).Results().First()
				.Should().Be.EqualTo(childVer1);

			AuditReader().CreateQuery().ForRevisionsOfEntity(typeof (ChildEntity), true, false).GetResultList<ChildEntity>()
				.Should().Have.SameValuesAs(childVer1, childVer2);
			AuditReader().CreateQuery().ForRevisionsOfEntity(typeof(ParentEntity), true, false).GetResultList<ParentEntity>()
				.Should().Have.SameValuesAs(childVer1, childVer2, parentVer1, parentVer2);
		}
	}
}