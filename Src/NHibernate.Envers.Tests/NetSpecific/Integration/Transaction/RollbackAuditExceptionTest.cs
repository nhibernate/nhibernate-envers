using System.Collections.Generic;
using System.Data.SqlClient;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Transaction
{
	[TestFixture, Ignore("Continue later - NHE7")]
	public class RollbackAuditExceptionTest : TestBase
	{
		protected override void Initialize()
		{
			dropStrTestAuditTable();
		}

		[Test]
		public void WhenAuditPeristExceptionOccursTransactionShouldBeRolledBack()
		{
			int intId;
			var strEntity = new StrTestEntity();
			var intEntity = new IntTestEntity();

			using (var tx = Session.BeginTransaction())
			{
				intId = (int)Session.Save(intEntity);
				Session.Save(strEntity);
				Assert.Throws<SqlException>(tx.Commit);
			}
			verifyNoDataGotPeristed(intId);
		}

		[Test]
		public void WhenAuditPeristExceptionOccursTransactionShouldBeRolledBack_FlushModeNever()
		{
			int intId;
			var strEntity = new StrTestEntity();
			var intEntity = new IntTestEntity();
			Session.FlushMode = FlushMode.Never;

			using (var tx = Session.BeginTransaction())
			{
				intId = (int)Session.Save(intEntity);
				Session.Save(strEntity);
				Assert.Throws<SqlException>(tx.Commit);
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
				Session.Auditer().GetRevisions<IntTestEntity>(id)
					.Should().Be.Empty();
			}
		}

		private void dropStrTestAuditTable()
		{
			Session.CreateSQLQuery("drop table StrTestEntity_AUD").ExecuteUpdate();
		}


		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml"};
			}
		}
	}
}