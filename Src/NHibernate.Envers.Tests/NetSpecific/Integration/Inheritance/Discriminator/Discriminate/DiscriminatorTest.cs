using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Inheritance.Discriminator.Discriminate
{
	/// <summary>
	/// Provides a basic set of tests to verify when the insert attribute of the discriminator element is set
	/// to false, that the setting will carry into the audit table settings. 
	/// </summary>
	public partial class DiscriminatorTest : TestBase
	{
		private BaseEntity baseEntityVer1;
		private BaseEntity baseEntityVer2;
		private SubtypeEntity subtypeEntityVer1;
		private SubtypeEntity subtypeEntityVer2;

		public DiscriminatorTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		/// <summary>
		/// Perform initialization for the test
		/// </summary>
		protected override void Initialize()
		{
			var baseEntityType = new ClassTypeEntity { Type = ClassTypeEntity.BaseName };
			var subtypeEntityType = new ClassTypeEntity { Type = ClassTypeEntity.SubtypeName };

			//rev 1
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(baseEntityType);
				Session.Save(subtypeEntityType);
				tx.Commit();
			}

			//rev 2
			var baseEntity = new BaseEntity { TypeId = baseEntityType, Data = "parent data" };
			var subtypeEntity = new SubtypeEntity { TypeId = subtypeEntityType, Data = "child data", SubtypeData = "child specific data" };
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(baseEntity);
				Session.Save(subtypeEntity);
				tx.Commit();
			}

			//rev 3
			using (var tx = Session.BeginTransaction())
			{
				baseEntity.Data = "parent data modified";
				subtypeEntity.Data = "child data modified";
				tx.Commit();
			}

			baseEntityVer1 = new BaseEntity { Id = baseEntity.Id, TypeId = baseEntityType, Data = "parent data" };
			subtypeEntityVer1 = new SubtypeEntity {Id = subtypeEntity.Id, TypeId = subtypeEntityType, Data = "child data", SubtypeData = "child specific data"};
			baseEntityVer2 = new BaseEntity {Id = baseEntity.Id, TypeId = baseEntityType, Data = "parent data modified"};
			subtypeEntityVer2 = new SubtypeEntity { Id = subtypeEntity.Id, TypeId = subtypeEntityType, Data = "child data modified", SubtypeData = "child specific data" };
		}

		/// <summary>
		/// This test is here to verify that the NHibernate configuration was able to complete and that the set up
		/// performed in the Initialize method is correct.
		/// </summary>
		[Test]
		public void VerifyInitializeCompleted()
		{
			var classTypeEntityCount = Session.Query<ClassTypeEntity>().Count();
			Assert.IsTrue(classTypeEntityCount == 2, "Did not retrieve two ClassTypeEntity records, but received {0}", classTypeEntityCount);

			var baseTypeCount = Session.Query<BaseEntity>()
				.Where(be => be.TypeId.Id == 1)
				.Count();
			Assert.IsTrue(baseTypeCount == 1, "Only expected one BaseEntity record, but received {0}", baseTypeCount);

			var subtypeCount = Session.Query<SubtypeEntity>().Count();
			Assert.IsTrue(subtypeCount == 1, "Only expected one SubtypeEntity record, but received {0}", subtypeCount);
		}

		/// <summary>
		/// Verify that revisions match for each type 
		/// </summary>
		[Test]
		public void VerifyRevisionCounts()
		{
			CollectionAssert.AreEquivalent(new[] { 2, 3 },
										   AuditReader().GetRevisions(typeof(BaseEntity), baseEntityVer1.Id));
			CollectionAssert.AreEquivalent(new[] { 2, 3 },
										   AuditReader().GetRevisions(typeof(SubtypeEntity), subtypeEntityVer1.Id));
		}

		[Test]
		public void VerifyHistoryOfParent()
		{
			AuditReader().Find<BaseEntity>(baseEntityVer1.Id, 2)
				.Should().Be.EqualTo(baseEntityVer1);

			AuditReader().Find<BaseEntity>(baseEntityVer2.Id, 3)
				.Should().Be.EqualTo(baseEntityVer2);
		}

		[Test]
		public void VerifyHistoryOfChild()
		{
			AuditReader().Find<SubtypeEntity>(subtypeEntityVer1.Id, 2)
				.Should().Be.EqualTo(subtypeEntityVer1);

			AuditReader().Find<SubtypeEntity>(subtypeEntityVer2.Id, 3)
				.Should().Be.EqualTo(subtypeEntityVer2);
		}
	}
}