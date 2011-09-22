using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.PropertyNull
{
	[TestFixture]
	public class PrimitiveNullTest : TestBase
	{
		private int id;

		protected override void Initialize()
		{
			var entity = new EntityWithPrimitiveNullAsCamelcaseUnderscore{Value = 1};

			using (var tx = Session.BeginTransaction())
			{
				id = (int) Session.Save(entity);
				tx.Commit();
			}
		}

		[Test]
		public void ShouldBeAbleToFindAuditedEntity()
		{
			var rev = AuditReader().Find<EntityWithPrimitiveNullAsCamelcaseUnderscore>(id, 1);
			rev.Id.Should().Be.EqualTo(id);
			rev.Value.Should().Be.EqualTo(1);
		}
	}
}