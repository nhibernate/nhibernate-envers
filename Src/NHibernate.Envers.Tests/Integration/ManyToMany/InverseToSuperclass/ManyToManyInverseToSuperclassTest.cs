using System.Collections.Generic;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ManyToMany.InverseToSuperclass
{
	public partial class ManyToManyInverseToSuperclassTest : TestBase
	{
		private long masterId;

		public ManyToManyInverseToSuperclassTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var master = new Master();
			var det1 = new DetailSubclass2();

			using (var tx = Session.BeginTransaction())
			{
				det1.Str2 = "detail 1";
				master.Str = "master";
				master.Items = new List<DetailSubclass> {det1};
				det1.Masters = new List<Master> {master};
				masterId = (long) Session.Save(master);
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