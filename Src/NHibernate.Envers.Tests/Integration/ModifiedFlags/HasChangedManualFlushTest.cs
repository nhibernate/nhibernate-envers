using System.Collections.Generic;
using NHibernate.Envers.Tests.Integration.Basic;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public partial class HasChangedManualFlushTest : AbstractModifiedFlagsEntityTest
	{
		private int id;

		public HasChangedManualFlushTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var entity = new BasicTestEntity1 { Long1 = 1, Str1 = "str1" };
			
			//Revision 1
			using (var tx = Session.BeginTransaction())
			{
				id = (int) Session.Save(entity);
				tx.Commit();
			}

			ForceNewSession();

			//Revision 2 - both properties (str1 and long1) should be marked as modified
			using (var tx = Session.BeginTransaction())
			{
				entity.Str1 = "str2";
				entity = Session.Merge(entity);
				Session.Flush();

				entity.Long1 = 2;
				Session.Flush();

				tx.Commit();
			}
		}

		[Test]
		public void ShouldHaveChangedOnDoubleFlush()
		{
			var list = QueryForPropertyHasChanged(typeof (BasicTestEntity1), id, "Str1");
			list.Count.Should().Be.EqualTo(2);
			list.ExtractRevisionNumbersFromRevision().Should().Have.SameSequenceAs(1, 2);

			list = QueryForPropertyHasChanged(typeof (BasicTestEntity1), id, "Long1");
			list.Count.Should().Be.EqualTo(2);
			list.ExtractRevisionNumbersFromRevision().Should().Have.SameSequenceAs(1, 2);
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