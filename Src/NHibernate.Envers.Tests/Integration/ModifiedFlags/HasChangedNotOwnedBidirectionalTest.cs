using System.Collections.Generic;
using NHibernate.Envers.Tests.Integration.Inheritance.Entities;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public partial class HasChangedNotOwnedBidirectionalTest : AbstractModifiedFlagsEntityTest
	{
		private long pc_id;
		private long a1_id;
		private long a2_id;

		public HasChangedNotOwnedBidirectionalTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			pc_id = 1;
			a1_id = 10;
			a2_id = 100;

			var pc = new PersonalContact{Id = pc_id, Email = "e", FirstName = "f"};
			var a1 = new Address {Id = a1_id, Address1 = "a1", Contact = pc};

			//rev1
			using (var tx = Session.BeginTransaction())
			{
				pc_id = (long) Session.Save(pc);
				a1_id = (long) Session.Save(a1);
				tx.Commit();
			}

			//rev2
			using (var tx = Session.BeginTransaction())
			{
				var a2 = new Address {Id = a2_id, Address1 = "a2", Contact = pc};
				a2_id = (long) Session.Save(a2);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyReferencedEntityHasChanged()
		{
			QueryForPropertyHasChanged(typeof (PersonalContact), pc_id, "Addresses")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1, 2);
			QueryForPropertyHasChanged(typeof(Address), a1_id, "Contact")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1);
			QueryForPropertyHasChanged(typeof(Address), a2_id, "Contact")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(2);

		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Integration.Inheritance.Joined.NotOwnedRelation.Mapping.hbm.xml" };
			}
		}
	}
}