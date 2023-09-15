using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Mixed
{
	public partial class MixedInheritanceStrategiesEntityTest : TestBase
	{
		private ActivityId id1;
		private ActivityId id2;
		private ActivityId id3;

		public MixedInheritanceStrategiesEntityTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			id1 = new ActivityId { Id1 = 1, Id2 = 2 };
			id2 = new ActivityId {Id1 = 2, Id2 = 3};
			id3 = new ActivityId {Id1 = 3, Id2 = 4};

			var normalActivity = new NormalActivity { Id = id1, SequenceNumber = 1 };
			var checkInActivity = new CheckInActivity
			{
				Id = id2,
				SequenceNumber = 0,
				DurationInMinutes = 30,
				RelatedActivity = normalActivity
			};

			// Revision 1
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(normalActivity);
				tx.Commit();
			}

			//Revision 2
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(checkInActivity);
				tx.Commit();
			}

			//revision 3
			using (var tx = Session.BeginTransaction())
			{
				normalActivity = new NormalActivity {Id = id3, SequenceNumber = 2};
				Session.Save(normalActivity);
				tx.Commit();
			}

			//revision 4
			using (var tx = Session.BeginTransaction())
			{
				checkInActivity.RelatedActivity = normalActivity;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionsCounts()
		{
			AuditReader().GetRevisions(typeof (NormalActivity), id1)
				.Should().Have.SameSequenceAs(1);
			AuditReader().GetRevisions(typeof(NormalActivity), id3)
				.Should().Have.SameSequenceAs(3);
			AuditReader().GetRevisions(typeof(CheckInActivity), id2)
				.Should().Have.SameSequenceAs(2, 4);
		}

		[Test]
		public void VerifyCurrentStateOfCheckInActivity()
		{
			var checkinActivity = Session.Get<CheckInActivity>(id2);
			var normalActivity = Session.Get<NormalActivity>(id3);

			checkinActivity.Id.Should().Be.EqualTo(id2);
			checkinActivity.SequenceNumber.Should().Be.EqualTo(0);
			checkinActivity.DurationInMinutes.Should().Be.EqualTo(30);
			checkinActivity.RelatedActivity.Id.Should().Be.EqualTo(normalActivity.Id);
			checkinActivity.RelatedActivity.SequenceNumber.Should().Be.EqualTo(normalActivity.SequenceNumber);
		}

		[Test]
		public void VerifyCurrentStateOfNormalActivities()
		{
			var normalActivity1 = Session.Get<NormalActivity>(id1);
			var normalActivity2 = Session.Get<NormalActivity>(id3);

			normalActivity1.Id.Should().Be.EqualTo(id1);
			normalActivity1.SequenceNumber.Should().Be.EqualTo(1);
			normalActivity2.Id.Should().Be.EqualTo(id3);
			normalActivity2.SequenceNumber.Should().Be.EqualTo(2);
		}

		[Test]
		public void VerifyFirstRevisionOfCheckInActivity()
		{
			var checkInActivity = AuditReader().Find<CheckInActivity>(id2, 2);
			var normalActivity = AuditReader().Find<NormalActivity>(id1, 2);

			checkInActivity.Id.Should().Be.EqualTo(id2);
			checkInActivity.SequenceNumber.Should().Be.EqualTo(0);
			checkInActivity.DurationInMinutes.Should().Be.EqualTo(30);
			checkInActivity.RelatedActivity.Id.Should().Be.EqualTo(normalActivity.Id);
			checkInActivity.RelatedActivity.SequenceNumber.Should().Be.EqualTo(normalActivity.SequenceNumber);
		}

		[Test]
		public void VerifySecondRevisionOfCheckInActivity()
		{
			var checkInActivity = AuditReader().Find<CheckInActivity>(id2, 4);
			var normalActivity = AuditReader().Find<NormalActivity>(id3, 4);

			checkInActivity.Id.Should().Be.EqualTo(id2);
			checkInActivity.SequenceNumber.Should().Be.EqualTo(0);
			checkInActivity.DurationInMinutes.Should().Be.EqualTo(30);
			checkInActivity.RelatedActivity.Id.Should().Be.EqualTo(normalActivity.Id);
			checkInActivity.RelatedActivity.SequenceNumber.Should().Be.EqualTo(normalActivity.SequenceNumber);
		}
	}
}