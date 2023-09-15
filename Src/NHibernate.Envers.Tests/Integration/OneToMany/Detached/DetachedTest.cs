using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.OneToMany.Detached;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.OneToMany.Detached
{
	public partial class DetachedTest : TestBase
	{
		private const int parentId = 132;
		private int childId;

		public DetachedTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var child = new StrTestEntity { Str = "data" };
			var parent = new ListRefCollEntity { Id = parentId, Data = "initial data", Collection = new List<StrTestEntity>{child}};

			//Revision 1
			using (var tx = Session.BeginTransaction())
			{
				childId = (int) Session.Save(child);
				Session.Save(parent);
				tx.Commit();
			}
			ForceNewSession();
			//Revision 2
			using (var tx = Session.BeginTransaction())
			{
				parent.Data = "modified data";
				Session.Update(parent);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			AuditReader().GetRevisions(typeof (ListRefCollEntity), parentId).Should().Have.SameSequenceAs(1, 2);
			AuditReader().GetRevisions(typeof (StrTestEntity), childId).Should().Have.SameSequenceAs(1);
		}

		[Test]
		public void VerifyHistoryOfParent()
		{
			var parent = new ListRefCollEntity
			             	{
			             		Id = parentId,
			             		Data = "initial data",
			             		Collection = new List<StrTestEntity> {new StrTestEntity {Id = childId, Str = "data"}}
			             	};
			
			var ver1 = AuditReader().Find<ListRefCollEntity>(parentId, 1);
			ver1.Should().Be.EqualTo(parent);
			ver1.Collection.Should().Have.SameValuesAs(parent.Collection);

			parent.Data = "modified data";
			var ver2 = AuditReader().Find<ListRefCollEntity>(parentId, 2);
			ver2.Should().Be.EqualTo(parent);
			ver1.Collection.Should().Have.SameValuesAs(parent.Collection);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.OneToMany.Detached.Mapping.hbm.xml", "Entities.Mapping.hbm.xml" };
			}
		}
	}
}