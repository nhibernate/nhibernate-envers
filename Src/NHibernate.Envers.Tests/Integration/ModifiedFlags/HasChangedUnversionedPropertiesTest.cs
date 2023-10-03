using System.Collections.Generic;
using NHibernate.Envers.Tests.Integration.Basic;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public partial class HasChangedUnversionedPropertiesTest : AbstractModifiedFlagsEntityTest
	{
		private int id;

		public HasChangedUnversionedPropertiesTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var entity = new BasicTestEntity2 {Str1 = "x", Str2 = "a"};
			using (var tx = Session.BeginTransaction())
			{
				id = (int) Session.Save(entity);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				entity.Str1 = "x";
				entity.Str2 = "a";
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				entity.Str1 = "y";
				entity.Str2 = "b";
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				entity.Str1 = "y";
				entity.Str2 = "b";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyHasChangedQuery()
		{
			QueryForPropertyHasChanged(typeof (BasicTestEntity2), id, "Str1")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1, 2);
		}

		[Test]
		public void ShouldThrowExceptionOnHasChangedQuery()
		{
			Assert.Throws<QueryException>(() =>
			         QueryForPropertyHasChangedWithDeleted(typeof (BasicTestEntity2), id, "Str2"));
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