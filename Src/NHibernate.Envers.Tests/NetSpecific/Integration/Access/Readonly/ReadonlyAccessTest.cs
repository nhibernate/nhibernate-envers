using System.Collections;
using NHibernate.Envers.Query;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Access.Readonly
{
	public partial class ReadonlyAccessTest : TestBase
	{
		private int id;

		public ReadonlyAccessTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ent = new ReadonlyEntity{Data=1};
			ent.SetReadOnlyData(1);

			using (var tx = Session.BeginTransaction())
			{
				id = (int) Session.Save(ent);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				ent.Data = 2;
				ent.SetReadOnlyData(2);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			CollectionAssert.AreEqual(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(ReadonlyEntity), id));
		}

		[Test]
		public void VerifyHistoryOfComponent()
		{
			var ver1 = AuditReader().Find<ReadonlyEntity>(id, 1);
			var ver2 = AuditReader().Find<ReadonlyEntity>(id, 2);

			ver1.Data.Should().Be.EqualTo(1);
			ver1.ReadonlyData.Should().Be.EqualTo(0);

			ver2.Data.Should().Be.EqualTo(2);
			ver2.ReadonlyData.Should().Be.EqualTo(0);
		}

		[Test]
		public void CanQueryOnReadonlyData()
		{
			var res = (IList)AuditReader().CreateQuery()
				.ForRevisionsOfEntity(typeof (ReadonlyEntity), false, false)
				.Add(AuditEntity.Property("ReadonlyData").Eq(2))
				.GetSingleResult();

			var ent = (ReadonlyEntity) res[0];
			var rev = (DefaultRevisionEntity) res[1];

			rev.Id.Should().Be.EqualTo(2);
			ent.Data.Should().Be.EqualTo(2);
			ent.ReadonlyData.Should().Be.EqualTo(0);
		}
	}
}