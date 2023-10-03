using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.ForceInitialize
{
	public partial class ForceInitializeTest : TestBase
	{

		private Guid parentId;
		private Guid child1Id;
		private Guid relatedId;

		public ForceInitializeTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var parent = new Parent { Children = new HashSet<Child>(), Data = 1 };


			//revision 1
			using (var tx = Session.BeginTransaction())
			{
				parentId = (Guid)Session.Save(parent);
				tx.Commit();
			}

			//revision 2
			using (var tx = Session.BeginTransaction())
			{
				var relatedEntity = new Related {Str = "R_1_1"};
				var strTestEntity = new Child { Str = "1_1" };
				parent.RelatedEntity = relatedEntity;
				parent.Children.Add(strTestEntity);
				tx.Commit();
				child1Id = strTestEntity.Id;
				relatedId = relatedEntity.Id;
			}
			// revision 3
			using (var tx = Session.BeginTransaction())
			{
				parent.RelatedEntity.Str = "R_1_2";
				parent.Children.First().Str = "1_2";
				tx.Commit();
			}
		}

		[Test]
		public void InitializeManyToOneNull()
		{
			var parent = AuditReader().Find<Parent>(parentId, 1);
			NHibernateUtil.IsInitialized(parent.RelatedEntity).Should().Be.True();
			parent.RelatedEntity.Should().Be.Null();
		}

		[Test]
		public void InitializeManyToOneNotNull()
		{
			var parent = AuditReader().Find<Parent>(parentId, 2);
			NHibernateUtil.IsInitialized(parent.RelatedEntity).Should().Be.False();
			NHibernateUtil.Initialize(parent.RelatedEntity);
			NHibernateUtil.IsInitialized(parent.RelatedEntity).Should().Be.True();
			parent.RelatedEntity.Id.Should().Be.EqualTo(relatedId);
			parent.RelatedEntity.Str.Should().Be.EqualTo("R_1_1");
		}
		
		[Test]
		public void InitializeManyToOneNotNullAsync()
		{
			var parent = AuditReader().Find<Parent>(parentId, 2);
			NHibernateUtil.IsInitialized(parent.RelatedEntity).Should().Be.False();
			NHibernateUtil.InitializeAsync(parent.RelatedEntity);
			NHibernateUtil.IsInitialized(parent.RelatedEntity).Should().Be.True();
			parent.RelatedEntity.Id.Should().Be.EqualTo(relatedId);
			parent.RelatedEntity.Str.Should().Be.EqualTo("R_1_1");
		}

		[Test]
		public void InitializeOneToManyEmpty()
		{
			var parent = AuditReader().Find<Parent>(parentId, 1);

			NHibernateUtil.IsInitialized(parent.Children).Should().Be.False();
			NHibernateUtil.Initialize(parent.Children);
			NHibernateUtil.IsInitialized(parent.Children).Should().Be.True();

			parent.Children.Should().Be.Empty();
		}

		[Test]
		public void InitializeOneToManyNotEmpty()
		{
			var parent = AuditReader().Find<Parent>(parentId, 3);

			NHibernateUtil.IsInitialized(parent.Children).Should().Be.False();
			NHibernateUtil.Initialize(parent.Children);
			NHibernateUtil.IsInitialized(parent.Children).Should().Be.True();

			var children = parent.Children.ToList();
			children.Should().Have.Count.EqualTo(1);
			children[0].Id.Should().Be.EqualTo(child1Id);
			children[0].Str.Should().Be.EqualTo("1_2");
		}

		[Test]
		public void CanReuseCollectionProxyAsNormalEntity()
		{			
			using (var tx = Session.BeginTransaction())
			{
				var ver3 = AuditReader().Find<Parent>(parentId, 3);
				ver3.Children.First().Str = "1_3";
				var newChild = new Child { Str = "2_1" };
				ver3.Children.Add(newChild);
				Session.Merge(ver3);
				tx.Commit();
			}
			using (Session.BeginTransaction())
			{
				var afterMerge = Session.Get<Parent>(parentId);
				afterMerge.Children.Count.Should().Be.EqualTo(2);
				afterMerge.Children.First().Id.Should().Be.EqualTo(child1Id);
				afterMerge.Children.First().Str.Should().Be.EqualTo("1_3");
				afterMerge.Children.Last().Str.Should().Be.EqualTo("2_1");
			}
		}

		[Test]
		public void NoUpdatesIfMergingNonChangingEntity()
		{
			using (var tx = Session.BeginTransaction())
			{
				var ver3 = AuditReader().Find<Parent>(parentId, 3);
				Session.Merge(ver3);
				Session.SessionFactory.Statistics.Clear();
				tx.Commit();

				Session.SessionFactory.Statistics.PrepareStatementCount
					.Should().Be.EqualTo(0);
			}
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.Properties[NHibernate.Cfg.Environment.GenerateStatistics] = "true";
		}
	}
}
