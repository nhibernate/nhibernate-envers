using System;
using System.Collections.Generic;
using System.Threading;
using NHibernate.Dialect;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.RevForDate
{
	public class RevisionForDateTest : TestBase
	{
		private DateTime timestamp1;
		private DateTime timestamp2;
		private DateTime timestamp3;
		private DateTime timestamp4;

		public RevisionForDateTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml"};
			}
		}

		protected override void Initialize()
		{
			var rfd = new StrTestEntity { Str = "x" };

			timestamp1 = DateTime.UtcNow.AddMilliseconds(-100);

			using (var tx = Session.BeginTransaction())
			{
				Session.Save(rfd);
				tx.Commit();
			}

			timestamp2 = DateTime.UtcNow;
			Thread.Sleep(100);
			using (var tx = Session.BeginTransaction())
			{
				rfd.Str = "y";
				tx.Commit();
			}
			timestamp3 = DateTime.UtcNow;
			Thread.Sleep(100);
			using (var tx = Session.BeginTransaction())
			{
				rfd.Str = "z";
				tx.Commit();
			}
			timestamp4 = DateTime.UtcNow;
		}

		[Test]
		public void TooEarlyTimeStampShouldFireException()
		{
			Assert.That(() => AuditReader().GetRevisionNumberForDate(timestamp1), Throws.TypeOf<RevisionDoesNotExistException>());
		}

		[Test]
		public void VerifyTimestamps()
		{
			Assert.AreEqual(1, AuditReader().GetRevisionNumberForDate(timestamp2));
			Assert.AreEqual(2, AuditReader().GetRevisionNumberForDate(timestamp3));
			Assert.AreEqual(3, AuditReader().GetRevisionNumberForDate(timestamp4));
		}


		[Test]
		public void VerifyDatesForRevisions()
		{
			Assert.AreEqual(1, AuditReader().GetRevisionNumberForDate(AuditReader().GetRevisionDate(1)));
			Assert.AreEqual(2, AuditReader().GetRevisionNumberForDate(AuditReader().GetRevisionDate(2)));
			Assert.AreEqual(3, AuditReader().GetRevisionNumberForDate(AuditReader().GetRevisionDate(3)));
		}

		[Test]
		public void VerifyRevisionsForDates()
		{
			var ar = AuditReader();

			ar.GetRevisionDate(ar.GetRevisionNumberForDate(timestamp2)).LessOrEqualTo(timestamp2);
			ar.GetRevisionDate(ar.GetRevisionNumberForDate(timestamp2) + 1).GreaterThan(timestamp2);
			ar.GetRevisionDate(ar.GetRevisionNumberForDate(timestamp3)).LessOrEqualTo(timestamp3);
			ar.GetRevisionDate(ar.GetRevisionNumberForDate(timestamp3) + 1).GreaterThan(timestamp3);
			ar.GetRevisionDate(ar.GetRevisionNumberForDate(timestamp4)).LessOrEqualTo(timestamp4);
		}

		protected override string TestShouldNotRunMessage()
		{
			return Dialect is MySQLDialect ? "Not applicable for MySQL due to low datetime precision" : null;
		}
	}
}