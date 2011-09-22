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
			var entity = new EntityWithPrimitiveNullAsCamelcaseUnderscore();

			using (var tx = Session.BeginTransaction())
			{
				id = (int) Session.Save(entity);
				tx.Commit();
			}
		}

		[Test]
		public void ShouldBeAbleToFindAuditedEntity()
		{
			AuditReader().Find<EntityWithPrimitiveNullAsCamelcaseUnderscore>(id, 1).Id
				.Should().Be.EqualTo(id);
		}
	}
}