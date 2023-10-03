using System.Collections.Generic;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Integration.Basic;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public partial class HasChangedNullPropertiesTest : AbstractModifiedFlagsEntityTest
	{
		private int id1;
		private int id2;

		public HasChangedNullPropertiesTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var bte1 = new BasicTestEntity1 { Str1 = "x", Long1 = 1 };
			var bte2 = new BasicTestEntity1 { Str1 = null, Long1 = 20 };

			using (var tx = Session.BeginTransaction())
			{
				id1 = (int)Session.Save(bte1);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				id2 = (int)Session.Save(bte2);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				bte1.Str1 = null;	
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				bte2.Str1 = "y2";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyHasChanged()
		{
			QueryForPropertyHasChangedWithDeleted(typeof (BasicTestEntity1), id1, "Str1")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1, 3);
			QueryForPropertyHasChangedWithDeleted(typeof(BasicTestEntity1), id1, "Long1")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1);
			// str1 property was null before insert and after insert so in a way it didn't change - is it a good way to go?
			QueryForPropertyHasChangedWithDeleted(typeof(BasicTestEntity1), id2, "Str1")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(4);
			QueryForPropertyHasChangedWithDeleted(typeof(BasicTestEntity1), id2, "Long1")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(2);

			AuditReader().CreateQuery().ForRevisionsOfEntity(typeof (BasicTestEntity1), false, true)
				.Add(AuditEntity.Property("Str1").HasChanged())
				.Add(AuditEntity.Property("Long1").HasChanged())
				.GetResultList()
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Integration.Basic.Mapping.hbm.xml" };
			}
		}
	}
}