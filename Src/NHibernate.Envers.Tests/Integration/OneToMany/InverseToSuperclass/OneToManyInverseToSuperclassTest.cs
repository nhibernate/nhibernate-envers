using System.Collections.Generic;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.OneToMany.InverseToSuperclass
{
	public partial class OneToManyInverseToSuperclassTest : TestBase
	{
		private long masterId;

		public OneToManyInverseToSuperclassTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var master = new Master();
			DetailSubclass det1 = new DetailSubclass2();
			DetailSubclass det2 = new DetailSubclass2();

			//revision 1
			using (var tx = Session.BeginTransaction())
			{
				det1.Str2 = "detail 1";
				master.Str = "master";
				master.Items = new List<DetailSubclass> {det1};
				det1.Master = master;
				masterId = (long) Session.Save(master);
				tx.Commit();
			}

			//revision 2
			using (var tx = Session.BeginTransaction())
			{
				det2.Str2 = "detail 2";
				det2.Master = master;
				master.Items.Add(det2);
				tx.Commit();
			}

			//revision 3
			using (var tx = Session.BeginTransaction())
			{
				master.Str = "new master";
				det1.Str2 = "new detail";
				var det3 = new DetailSubclass2 {Str2 = "detail 3", Master = master};
				master.Items[1].Master = null;
				master.Items.Add(det3);
				tx.Commit();
			}

			//revision 4
			using (var tx = Session.BeginTransaction())
			{
				det1 = master.Items[0];
				det1.Master = null;
				tx.Commit();
			}
		}

		[Test]
		public void HistoryShouldExist()
		{
			var rev1 = AuditReader().Find<Master>(masterId, 1);
			var rev2 = AuditReader().Find<Master>(masterId, 2);
			var rev3 = AuditReader().Find<Master>(masterId, 3);
			var rev4 = AuditReader().Find<Master>(masterId, 4);

			rev1.Should().Not.Be.Null();
			rev2.Should().Not.Be.Null();
			rev3.Should().Not.Be.Null();
			rev4.Should().Not.Be.Null();
		}
	}
}