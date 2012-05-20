using System.Collections.Generic;
using System.Transactions;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Transaction
{
	public class SessionClosedWhenCommitingTest : TestBase
	{
		private StrTestEntity entity;

		public SessionClosedWhenCommitingTest(string strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			entity = new StrTestEntity {Str = "data"};

			using (var distrTx = new TransactionScope())
			{
				using (var newSession = Session.SessionFactory.OpenSession())
				{
					newSession.FlushMode = FlushMode.Never;
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