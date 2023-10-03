using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.EnumType
{
	public partial class CustomEnumTypeTest : TestBase
	{
		private int id;

		public CustomEnumTypeTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var entity = new Entity{EntityEnum = EntityEnum.TypeB};
			using (var tx = Session.BeginTransaction())
			{
				id = (int) Session.Save(entity);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			AuditReader().GetRevisions(typeof(Entity), id)
				.Should().Have.SameSequenceAs(1);
		}

		[Test]
		public void VerifyHistoryOfEntity()
		{
			AuditReader().Find<Entity>(id, 1).EntityEnum.Should().Be.EqualTo(EntityEnum.TypeB);
		}
	}
}