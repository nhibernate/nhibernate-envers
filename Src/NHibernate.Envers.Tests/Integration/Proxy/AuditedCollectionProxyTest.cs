using NHibernate.Envers.Tests.Entities.OneToMany;
using NHibernate.Proxy;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Proxy
{
	public partial class AuditedCollectionProxyTest : TestBase
	{
		public AuditedCollectionProxyTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var refEntity = new SetRefEdEntity {Id = 1, Data = "str1"};
			var refingEntity = new SetRefIngEntity{Id = 1, Data = "refing1", Reference = refEntity};
			var refingEntity2 = new SetRefIngEntity { Id = 2, Data = "refing2", Reference = refEntity };

			//rev 1
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(refEntity);
				Session.Save(refingEntity);
				tx.Commit();
			}
			//rev 2
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(refingEntity2);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyProxyIdentifier()
		{
			var refEntity = Session.Load<SetRefEdEntity>(1);
			refEntity.Should().Be.InstanceOf<INHibernateProxy>();

			var refingEntity3 = new SetRefIngEntity { Id = 3, Data = "refing2", Reference = refEntity };

			//rev 3
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(refingEntity3);
				tx.Commit();
			}
		}

		protected override System.Collections.Generic.IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.OneToMany.Mapping.hbm.xml", "Entities.Mapping.hbm.xml" };
			}
		}
	}
}