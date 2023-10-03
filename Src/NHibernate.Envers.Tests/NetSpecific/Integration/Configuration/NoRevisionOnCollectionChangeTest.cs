using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tests.Entities.OneToMany.Detached;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Configuration
{
	public partial class NoRevisionOnCollectionChangeTest : TestBase
	{
		private int parentId;

		public NoRevisionOnCollectionChangeTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var parent = new IndexedListJoinColumnBidirectionalRefIngEntity{References = new List<IndexedListJoinColumnBidirectionalRefEdEntity>(), Data="data"};
			var child1 = new IndexedListJoinColumnBidirectionalRefEdEntity {Data = "data"};
			var child2 = new IndexedListJoinColumnBidirectionalRefEdEntity {Data = "data"};
			//only valid rev for parent
			using (var tx = Session.BeginTransaction())
			{
				parentId = (int) Session.Save(parent);
				Session.Save(child1);
				Session.Save(child2);
				tx.Commit();
			}
			//no rev for adding
			using (var tx = Session.BeginTransaction())
			{
				parent.References.Add(child1);
				parent.References.Add(child2);
				tx.Commit();
			}
			//no rev for modification
			using (var tx = Session.BeginTransaction())
			{
				parent.References.Clear();
				parent.References.Add(child2);
				parent.References.Add(child1);
				tx.Commit();
			}
			//no rev for delete
			using (var tx = Session.BeginTransaction())
			{
				parent.References.Clear();
				tx.Commit();
			}
		}

		[Test]
		public void ShouldOnlyCreatedOneRevisionForParent()
		{
			AuditReader().GetRevisions(typeof(IndexedListJoinColumnBidirectionalRefIngEntity), parentId)
				.Should().Have.Count.EqualTo(1);
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.RevisionOnCollectionChange, false);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.OneToMany.Detached.Mapping.hbm.xml", "Entities.Mapping.hbm.xml"};
			}
		}
	}
}