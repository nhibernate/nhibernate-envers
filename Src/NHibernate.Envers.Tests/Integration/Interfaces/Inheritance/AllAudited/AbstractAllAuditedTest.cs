using NHibernate.Envers.Exceptions;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Interfaces.Inheritance.AllAudited
{
	public abstract partial class AbstractAllAuditedTest : TestBase
	{
		private long aiId;
		private long naiId;

		protected AbstractAllAuditedTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ai = new AuditedImplementor {Data = "La data", AuditedImplementorData = "audited implementor data"};
			var nai = new NonAuditedImplementor {Data = "info", NonAuditedImplementorData = "string"};

			//Revision 1
			using (var tx = Session.BeginTransaction())
			{
				aiId = (long) Session.Save(ai);
				naiId = (long) Session.Save(nai);
				tx.Commit();
			}

			//Revision 2
			using (var tx = Session.BeginTransaction())
			{
				ai.Data = "La data 2";
				ai.AuditedImplementorData = "audited implementor data 2";
				nai.Data = "info 2";
				nai.NonAuditedImplementorData = "string 2";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisions()
		{
			AuditReader().GetRevisions(typeof (AuditedImplementor), aiId)
				.Should().Have.SameSequenceAs(1, 2);
		}

		[Test]
		public void VerifyRetrieveAudited()
		{
			var ai = Session.Get<AuditedImplementor>(aiId);
			var si = Session.Get<ISimple>(aiId);
			var aiRev1 = AuditReader().Find<AuditedImplementor>(aiId, 1);
			var siRev1 = AuditReader().Find<ISimple>(aiId, 1);
			var aiRev2 = AuditReader().Find<AuditedImplementor>(aiId, 2);
			var siRev2 = AuditReader().Find<ISimple>(aiId, 2);

			ai.Data.Should().Be.EqualTo("La data 2");
			si.Data.Should().Be.EqualTo("La data 2");
			aiRev1.Data.Should().Be.EqualTo("La data");
			siRev1.Data.Should().Be.EqualTo("La data");
			aiRev2.Data.Should().Be.EqualTo("La data 2");
			siRev2.Data.Should().Be.EqualTo("La data 2");
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