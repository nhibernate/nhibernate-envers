using System.Collections.Generic;
using System.Transactions;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Transaction
{
#if NETCOREAPP2_0_OR_GREATER
	[Ignore("System.Transactions is not supported in Core. See https://github.com/dotnet/corefx/issues/19894")]
#endif	
	public class SessionClosedWhenCommitingTest : TestBase
	{
		private StrTestEntity entity;

		public SessionClosedWhenCommitingTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			entity = new StrTestEntity {Str = "data"};

			using (var distrTx = new TransactionScope())
			{
				using (var newSession = Session.SessionFactory.OpenSession())
				{
					newSession.FlushMode = FlushMode.Manual;
					newSession.Save(entity);
					newSession.Flush();
				}
				distrTx.Complete();
			}
		}

		[Test]
		public void ShouldHaveCreatedRevision()
		{
			AuditReader().Find<StrTestEntity>(entity.Id, 1)
				.Should().Be.EqualTo(entity);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[]{"Entities.Mapping.hbm.xml"};
			}
		}
	}
}