using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Integration.Collection.MapKey;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public partial class HasChangedIdMapKeyTest : AbstractModifiedFlagsEntityTest
	{
		private int id;

		public HasChangedIdMapKeyTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var imke = new IdMapKeyEntity();
			var ste1 = new StrTestEntity { Str = "x" };
			var ste2 = new StrTestEntity { Str = "y" };

			// Revision 1 (intialy 1 mapping)
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ste1);
				Session.Save(ste2);
				imke.IdMap[ste1.Id] = ste1;
				id = (int) Session.Save(imke);
				tx.Commit();
			}

			// Revision 2 (sse1: adding 1 mapping)
			using (var tx = Session.BeginTransaction())
			{
				imke.IdMap[ste2.Id] = ste2;
				tx.Commit();
			}
		}

		[Test]
		public void VerifyHasChanged()
		{
			QueryForPropertyHasChanged(typeof (IdMapKeyEntity), id, "IdMap")
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1, 2);
			QueryForPropertyHasNotChanged(typeof(IdMapKeyEntity), id, "IdMap")
				.ExtractRevisionNumbersFromRevision()
				.Should().Be.Empty();
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[]
				       	{
				       		"Entities.Mapping.hbm.xml",
				       		"Integration.Collection.MapKey.Mapping.hbm.xml",
				       		"Entities.Components.Mapping.hbm.xml"
				       	};
			}
		}
	}
}