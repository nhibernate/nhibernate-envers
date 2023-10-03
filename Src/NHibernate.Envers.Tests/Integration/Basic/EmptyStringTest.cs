using System.Collections.Generic;
using NHibernate.Dialect;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Basic
{
	public partial class EmptyStringTest : TestBase
	{
		private int emptyId;
		private int nullId;

		public EmptyStringTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml" };
			}
		}

		protected override string TestShouldNotRunMessage()
		{
			return Dialect is Oracle8iDialect ? null : "Test only valid for Oracle";
		}

		protected override void Initialize()
		{
			var emptyEntity = new StrTestEntity { Str = "" };
			var nullEntity = new StrTestEntity { Str = null };

			//revision 1
			using (var tx = Session.BeginTransaction())
			{
				emptyId = (int)Session.Save(emptyEntity);
				nullId = (int)Session.Save(nullEntity);
				tx.Commit();
			}

			// Should not generate revision after NULL to "" modification and vice versa on Oracle.
			using (var tx = Session.BeginTransaction())
			{
				emptyEntity.Str = null;
				nullEntity.Str = "";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			AuditReader().GetRevisions(typeof (StrTestEntity), emptyId).Should().Have.SameSequenceAs(1);
			AuditReader().GetRevisions(typeof (StrTestEntity), nullId).Should().Have.SameSequenceAs(1);
		}
	}
}