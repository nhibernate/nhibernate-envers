using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Exceptions;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Transaction
{
	public partial class RollbackAuditExceptionTest : TestBase
	{
		public RollbackAuditExceptionTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		[Test]
		public void WhenAuditPersistExceptionOccursTransactionShouldBeRolledBack()
		{
			int intId;
			var willCrash = new NoSchemaEntity();
			var intEntity = new IntTestEntity();

			using (var tx = Session.BeginTransaction())
			{
				intId = (int)Session.Save(intEntity);
				Session.Save(willCrash);
				Assert.Throws<GenericADOException>(tx.Commit);
			}
			ForceNewSession();
			verifyNoDataGotPeristed(intId);
		}

		[Test]
		public void WhenAuditPersistExceptionOccursTransactionShouldBeRolledBack_FlushModeNever()
		{
			int intId;
			var willCrash = new NoSchemaEntity();
			var intEntity = new IntTestEntity();
			Session.FlushMode = FlushMode.Manual;

			using (Session.BeginTransaction())
			{
				intId = (int)Session.Save(intEntity);
				Session.Save(willCrash);
				Assert.Throws<GenericADOException>(Session.Flush);
			}
			verifyNoDataGotPeristed(intId);
		}

		private void verifyNoDataGotPeristed(int id)
		{
			using(Session.BeginTransaction())
			{
				Session.CreateQuery("select count(s) from StrTestEntity s ").UniqueResult<long>()
					.Should().Be.EqualTo(0);
				Session.CreateQuery("select count(s) from IntTestEntity s ").UniqueResult<long>()
					.Should().Be.EqualTo(0);			
				Session.Auditer().GetRevisions(typeof(IntTestEntity),id)
					.Should().Be.Empty();
			}
		}


		protected override void Initialize()
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml", "NetSpecific.Integration.Transaction.Mapping.hbm.xml"};
			}
		}
	}
}