using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Ids;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.OneToOne.UniDirectional
{
	public partial class UnidirectionalMulIdWithNullsTest : TestBase
	{
		private EmbId ei;

		public UnidirectionalMulIdWithNullsTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			ei = new EmbId {X = 1, Y = 2};

			var eite = new EmbIdTestEntity{Id = ei, Str1 = "data"};
			var notNullRef = new UniRefIngMulIdEntity{Id = 1, Data = "data 1", Reference = eite};
			var nullRef = new UniRefIngMulIdEntity{Id = 2, Data = "data 2", Reference = null};

			//Revision 1
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(eite);
				Session.Save(notNullRef);
				Session.Save(nullRef);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyNullReference()
		{
			AuditReader().Find<UniRefIngMulIdEntity>(2, 1).Reference
				.Should().Be.Null();
		}

		[Test]
		public void VerifyNotNullReference()
		{
			AuditReader().Find<UniRefIngMulIdEntity>(1, 1).Reference.Str1
				.Should().Be.EqualTo("data");
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Ids.Mapping.hbm.xml", "Integration.OneToOne.UniDirectional.Mapping.hbm.xml" };
			}
		}
	}
}