using NHibernate.Envers.Exceptions;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Interfaces.Inheritance.PropertiesAudited
{
	public abstract partial class AbstractAllAuditedTest : TestBase
	{
		private long aiId;
		private long naiId;

		private const int NUMBER = 555;

		protected AbstractAllAuditedTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ai = new AuditedImplementor {Data = "La data", AuditedImplementorData = "audited implementor data", Number = NUMBER};
			var nai = new NonAuditedImplementor { Data = "info", NonAuditedImplementorData = "string", Number = NUMBER };

			//Revision 1
			using (var tx = Session.BeginTransaction())
			{
				aiId = (long) Session.Save(ai);
				naiId = (long) Session.Save(nai);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRetrieveAudited()
		{
			var ai = Session.Get<AuditedImplementor>(aiId);
			var si = Session.Get<ISimple>(aiId);
			var aiRev1 = AuditReader().Find<AuditedImplementor>(aiId, 1);
			var siRev1 = AuditReader().Find<ISimple>(aiId, 1);

			ai.Data.Should().Be.EqualTo("La data");
			si.Data.Should().Be.EqualTo("La data");
			aiRev1.Data.Should().Be.Null();
			siRev1.Data.Should().Be.Null();
			siRev1.Number.Should().Be.EqualTo(NUMBER);
			siRev1.Number.Should().Be.EqualTo(NUMBER);
		}

		[Test]
		public void VerifyRetrieveNonAudited()
		{
			var nai = Session.Get<NonAuditedImplementor>(naiId);
			var si = Session.Get<ISimple>(naiId);

			si.Data.Should().Be.EqualTo(nai.Data);

			Assert.Throws<NotAuditedException>(() =>
			   AuditReader().Find<NonAuditedImplementor>(naiId, 1));

			AuditReader().Find<ISimple>(naiId, 1)
				.Should().Be.Null();
		}
	}
}