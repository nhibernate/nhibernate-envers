using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.InsertUpdateFalse
{
	public partial class InsertUpdateFalseComponentTest : TestBase
	{
		private int id;

		public InsertUpdateFalseComponentTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var entity = new ParentEntity { Component = new ChildComponent { NoUpdateInsert = 1 }, ComponentSetter = new ChildComponent()};

			//rev 1
			using (var tx = Session.BeginTransaction())
			{
				id = (int)Session.Save(entity);
				tx.Commit();
			}
			Session.Refresh(entity);
			using (var tx = Session.BeginTransaction())
			{
				entity.Component.NoUpdateInsert = 2;
				tx.Commit();
			}
			Session.Refresh(entity);
			//rev 2
			using (var tx = Session.BeginTransaction())
			{
				entity.ComponentSetter.NoUpdateInsert = 3;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(ParentEntity),id));
		}

		[Test]
		public void VerifyHistory()
		{
			var expected1 = new ParentEntity
			                	{
			                		Id = id,
			                		Component = new ChildComponent {NoUpdateInsert = 0},
			                		ComponentSetter = new ChildComponent {NoUpdateInsert = 0}
			                	};
			var expected2 = new ParentEntity
			                	{
			                		Id = id,
			                		Component = new ChildComponent {NoUpdateInsert = 3},
			                		ComponentSetter = new ChildComponent {NoUpdateInsert = 3}
			                	};

			var ver1 = AuditReader().Find<ParentEntity>(id, 1);
			var ver2 = AuditReader().Find<ParentEntity>(id, 2);

			ver1.Should().Be.EqualTo(expected1);
			ver2.Should().Be.EqualTo(expected2);
		}
	}
}