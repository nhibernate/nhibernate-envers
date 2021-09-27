using System;
using System.Collections.Generic;
using System.Threading;
using NHibernate.Dialect;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.RevEntity;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.RevEntity
{
	public class CustomDateTest : TestBase
	{
		private int id;
		private DateTime timestamp1;
		private DateTime timestamp2;
		private DateTime timestamp3;

		public CustomDateTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml", "Entities.RevEntity.CustomDateRevEntity.hbm.xml" };
			}
		}

		protected override void Initialize()
		{
			var te = new StrTestEntity { Str = "x" };

			timestamp1 = DateTime.UtcNow.AddMilliseconds(-100);
			using (var tx = Session.BeginTransaction())
			{
				id = (int)Session.Save(te);
				tx.Commit();
			}

			timestamp2 = DateTime.UtcNow;
			Thread.Sleep(100);
			using (var tx = Session.BeginTransaction())
			{
				te.Str = "y";
				tx.Commit();
			}
			timestamp3 = DateTime.UtcNow;
		}

		[Test]
		public void TooEarlyTimeStampShouldFireException()
		{
			Assert.That(()=>AuditReader().GetRevisionNumberForDate(timestamp1), Throws.TypeOf<RevisionDoesNotExistException>());
		}

		[Test]
		public void VerifyTimestamps()
		{
			Assert.AreEqual(1, AuditReader().GetRevisionNumberForDate(timestamp2));
			Assert.AreEqual(2, AuditReader().GetRevisionNumberForDate(timestamp3));
		}


		[Test]
		public void VerifyDatesForRevisions()
		{
			Assert.AreEqual(1, AuditReader().GetRevisionNumberForDate(AuditReader().GetRevisionDate(1)));
			Assert.AreEqual(2, AuditReader().GetRevisionNumberForDate(AuditReader().GetRevisionDate(2)));
		}

		[Test]
		public void VerifyRevisionsForDates()
		{
			var ar = AuditReader();

			ar.GetRevisionDate(ar.GetRevisionNumberForDate(timestamp2)).LessOrEqualTo(timestamp2);
			ar.GetRevisionDate(ar.GetRevisionNumberForDate(timestamp2) + 1).GreaterThan(timestamp2);
			ar.GetRevisionDate(ar.GetRevisionNumberForDate(timestamp3)).LessOrEqualTo(timestamp3);
		}

		[Test]
		public void VerifyFindRevision()
		{
			var rev1timestamp = AuditReader().FindRevision<CustomDateRevEntity>(1).DateTimestamp;
			rev1timestamp.GreaterThan(timestamp1);
			rev1timestamp.LessOrEqualTo(timestamp2);

			var rev2timestamp = ((CustomDateRevEntity)AuditReader().FindRevision(2)).DateTimestamp;
			rev2timestamp.GreaterThan(timestamp2);
			rev2timestamp.LessOrEqualTo(timestamp3);
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(StrTestEntity), id));
		}

		[Test]
		public void VerifyHistoryOfId()
		{
			var ver1 = new StrTestEntity { Id = id, Str = "x" };
			var ver2 = new StrTestEntity { Id = id, Str = "y" };

			Assert.AreEqual(ver1, AuditReader().Find<StrTestEntity>(id, 1));
			Assert.AreEqual(ver2, AuditReader().Find<StrTestEntity>(id, 2));
		}
		
		protected override string TestShouldNotRunMessage()
		{
			return Dialect is MySQLDialect ? "Not applicable for MySQL due to low datetime precision" : null;
		}
	}
}