using System;
using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Ids;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Ids
{
	public partial class CompositeDateIdTest : TestBase
	{
		private DateEmbId id;

		public CompositeDateIdTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			id = new DateEmbId{X = new DateTime(1900,1,1), Y=new DateTime(2000,1,1)};
			var dite = new CompositeDateIdTestEntity {Id = id, Str1 = "x"};
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(dite);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				dite.Str1 = "y";
				tx.Commit();
			}
		}


		[Test]
		public void VerifyRevisionCount()
		{
			AuditReader().GetRevisions(typeof (CompositeDateIdTestEntity), id)
				.Should().Have.SameSequenceAs(1, 2);
		}

		[Test]
		public void VerifyHistory()
		{
			AuditReader().Find<CompositeDateIdTestEntity>(id, 1).Str1
				.Should().Be.EqualTo("x");
			AuditReader().Find<CompositeDateIdTestEntity>(id, 2).Str1
				.Should().Be.EqualTo("y");
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Ids.Mapping.hbm.xml" }; }
		}

	}
}