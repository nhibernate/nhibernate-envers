using System;
using System.Collections.Generic;
using System.Threading;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.RevForDate
{
	[TestFixture]
	public class RevisionForDateTest : TestBase
	{
		private long timestamp1;
		private long timestamp2;
		private long timestamp3;
		private long timestamp4;

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

			timestamp1 = DateTime.Now.AddSeconds(-1).Ticks;
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(rfd);
				tx.Commit();
			}

			timestamp2 = DateTime.Now.Ticks;
			Thread.Sleep(100);
			using (var tx = Session.BeginTransaction())
			{
				rfd.Str = "y";
				tx.Commit();
			}
			timestamp3 = DateTime.Now.Ticks;
			Thread.Sleep(100);
			using (var tx = Session.BeginTransaction())
			{
				rfd.Str = "z";
				tx.Commit();
			}
			timestamp4 = DateTime.Now.Ticks;
		}

		[Test, ExpectedException(typeof(RevisionDoesNotExistException))]
		public void TooEarlyTimeStampShouldFireException()
		{
			AuditReader().GetRevisionNumberForDate(new DateTime(timestamp1));
		}

		[Test]
		public void VerifyTimestamps()
		{
			Assert.AreEqual(1, AuditReader().GetRevisionNumberForDate(new DateTime(timestamp2)));
			Assert.AreEqual(2, AuditReader().GetRevisionNumberForDate(new DateTime(timestamp3)));
			Assert.AreEqual(3, AuditReader().GetRevisionNumberForDate(new DateTime(timestamp4)));
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
			Assert.IsTrue(
				AuditReader().GetRevisionDate(AuditReader().GetRevisionNumberForDate(new DateTime(timestamp2))).Ticks <= timestamp2);
			Assert.IsTrue(
				AuditReader().GetRevisionDate(AuditReader().GetRevisionNumberForDate(new DateTime(timestamp2)) + 1).Ticks > timestamp2);
			Assert.IsTrue(
				AuditReader().GetRevisionDate(AuditReader().GetRevisionNumberForDate(new DateTime(timestamp3))).Ticks <= timestamp3);
			Assert.IsTrue(
				AuditReader().GetRevisionDate(AuditReader().GetRevisionNumberForDate(new DateTime(timestamp3)) + 1).Ticks > timestamp3);
			Assert.IsTrue(
				AuditReader().GetRevisionDate(AuditReader().GetRevisionNumberForDate(new DateTime(timestamp4))).Ticks <= timestamp4);
		}
	}
}