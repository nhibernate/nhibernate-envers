using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.OneToMany
{
	public partial class UniDirectionalWithBackRefTest : TestBase
	{
		private Guid parentId;
		private Guid child1Id;
		private Guid child2Id;

		public UniDirectionalWithBackRefTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var parent = new Parent {Children = new HashSet<Child>(), Data=1};
			

			//revision 1
			using (var tx = Session.BeginTransaction())
			{
				parentId = (Guid) Session.Save(parent);
				tx.Commit();
			}

			//revision 2
			using (var tx = Session.BeginTransaction())
			{
				var strTestEntity = new Child {Str = "1_1"};
				parent.Children.Add(strTestEntity);
				tx.Commit();
				child1Id = strTestEntity.Id;
			}

			//revision 3
			using (var tx = Session.BeginTransaction())
			{
				parent.Children.First().Str = "1_2";
				tx.Commit();
			}

			//Revision 4
			using (var tx = Session.BeginTransaction())
			{
				var strTestEntity = new Child { Str = "1_2"};
				parent.Children.Add(strTestEntity);
				parent.Data = 2;
				tx.Commit();
				child2Id = strTestEntity.Id;
			}

			//Revision 5
			using (var tx = Session.BeginTransaction())
			{
				parent.Children.Clear();
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			AuditReader().GetRevisions(typeof (Parent), parentId).Should().Have.SameSequenceAs(1, 2, 4, 5);
			AuditReader().GetRevisions(typeof(Child), child1Id).Should().Have.SameSequenceAs(2, 3, 5);
			AuditReader().GetRevisions(typeof(Child), child2Id).Should().Have.SameSequenceAs(4, 5);
		}

		[Test]
		public void VerifyHistoryOfParent()
		{
			var rev1 = AuditReader().Find<Parent>(parentId, 1);
			var rev2 = AuditReader().Find<Parent>(parentId, 2);
			var rev4 = AuditReader().Find<Parent>(parentId, 4);
			var rev5 = AuditReader().Find<Parent>(parentId, 5);

			rev1.Data.Should().Be.EqualTo(1);
			rev1.Children.Should().Be.Empty();
			rev2.Data.Should().Be.EqualTo(1);
			rev2.Children.Should().Have.Count.EqualTo(1);
			rev4.Data.Should().Be.EqualTo(2);
			rev4.Children.Should().Have.Count.EqualTo(2);
			rev5.Data.Should().Be.EqualTo(2);
			rev5.Children.Should().Be.Empty();
		}

		[Test]
		public void VerifyHistoryOfChild1()
		{
			var rev2 = AuditReader().Find<Child>(child1Id, 2);
			var rev3 = AuditReader().Find<Child>(child1Id, 3);
			var rev5 = AuditReader().Find<Child>(child1Id, 5);

			rev2.Str.Should().Be.EqualTo("1_1");
			rev3.Str.Should().Be.EqualTo("1_2");
			rev5.Should().Be.Null();
		}

		[Test]
		public void VerifyHistoryOfChild2()
		{
			var rev4 = AuditReader().Find<Child>(child2Id, 4);
			var rev5 = AuditReader().Find<Child>(child2Id, 5);

			rev4.Str.Should().Be.EqualTo("1_2");
			rev5.Should().Be.Null();
		}

		[Test, Ignore("Could be fixed but will be possible big, breaking change...")]
		public void NoExtraTableShouldHaveBeenCreated()
		{
			var persistenEnversClasses = Cfg.ClassMappings.Where(cm => cm.Table.Name.EndsWith("_AUD"));
			persistenEnversClasses.Should().Have.Count.EqualTo(2);
		}
	}
}