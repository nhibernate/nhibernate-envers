using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.NetSpecific.Entities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Proxy
{
	//tests TwoEntityQueryGenerator
	public partial class ListRemovedObjectQueryTest : TestBase
	{
		private Guid parentId;

		public ListRemovedObjectQueryTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var child1 = new ListChild {Name = "child1"};
			var parent = new ListParent {Children = new List<ListChild>{child1}};

			//Revision 1
			using (var tx = Session.BeginTransaction())
			{
				parentId = (Guid) Session.Save(parent);
				tx.Commit();
			}

			//Revision 2
			using (var tx = Session.BeginTransaction())
			{
				child1.Name = "child12";
				tx.Commit();
			}

			//Revision 3
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(parent);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyHistoryOfParent()
		{
			AuditReader().Find<ListParent>(parentId, 1)
						 .Children.Single().Name.Should().Be.EqualTo("child1");
			AuditReader().Find<ListParent>(parentId, 2)
						 .Children.Single().Name.Should().Be.EqualTo("child12");
			var rev3 = AuditReader().CreateQuery().ForRevisionsOfEntity(typeof(ListParent), true, true)
			             .Add(AuditEntity.Id().Eq(parentId))
			             .Add(AuditEntity.RevisionNumber().Eq(3))
			             .GetSingleResult<ListParent>();
			rev3.Children.Single().Name.Should().Be.EqualTo("child12");
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.StoreDataAtDelete, true);
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "NetSpecific.Entities.List.hbm.xml" }; }
		}
	}
}