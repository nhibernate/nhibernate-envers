using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.ManyToMany.UniDirectional;
using NHibernate.Envers.Tests.Entities.ManyToOne.UniDirectional;
using NHibernate.Envers.Tests.Entities.OneToMany;
using NHibernate.Proxy;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Proxy
{
	public partial class ProxyIdentifierTest : TestBase
	{
		private TargetNotAuditedEntity tnae1;
		private ManyToOneNotAuditedNullEntity mtonane1;
		private ManyToManyNotAuditedNullEntity mtmnane1;
		private OneToManyNotAuditedNullEntity otmnane1;
		private UnversionedStrTestEntity uste1;
		private UnversionedStrTestEntity uste2;

		public ProxyIdentifierTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[]
					{
						"Entities.Mapping.hbm.xml", 
						"Entities.ManyToOne.UniDirectional.Mapping.hbm.xml",
						"Entities.ManyToMany.UniDirectional.Mapping.hbm.xml",
						"Entities.OneToMany.Mapping.hbm.xml"
					};
			}
		}

		protected override void Initialize()
		{
			uste1 = new UnversionedStrTestEntity { Str = "str1" };
			uste2 = new UnversionedStrTestEntity { Str = "str2" };

			// No revision
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(uste1);
				Session.Save(uste2);
				tx.Commit();
			}

			// Revision 1
			using (var tx = Session.BeginTransaction())
			{
				tnae1 = new TargetNotAuditedEntity { Id = 1, Data = "tnae1", Reference = uste1 };
				Session.Save(tnae1);
				tx.Commit();
			}

			// Revision 2
			using (var tx = Session.BeginTransaction())
			{
				mtonane1 = new ManyToOneNotAuditedNullEntity(2, "mtonane1", uste2);
				mtmnane1 = new ManyToManyNotAuditedNullEntity(3, "mtmnane1");
				mtmnane1.References.Add(uste2);
				otmnane1 = new OneToManyNotAuditedNullEntity(4, "otmnane1");
				otmnane1.References.Add(uste2);
				Session.Save(mtonane1);
				Session.Save(mtmnane1);
				Session.Save(otmnane1);
				tx.Commit();
			}

			// Revision 3
			// Remove not audited target entity, so we can verify null reference
			// when "not-found=ignore" applied.
			using (var tx = Session.BeginTransaction())
			{
				mtonane1.Reference = null;
				mtmnane1.References = null;
				otmnane1.References = null;
				Session.Delete(uste2);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyProxyIdentifier()
		{
			var rev1 = AuditReader().Find<TargetNotAuditedEntity>(tnae1.Id, 1);
			var proxyCreatedByEnvers = rev1.Reference as INHibernateProxy;

			Assert.IsNotNull(proxyCreatedByEnvers);

			var lazyInitializer = proxyCreatedByEnvers.HibernateLazyInitializer;
			Assert.IsTrue(lazyInitializer.IsUninitialized);
			Assert.AreEqual(tnae1.Id, lazyInitializer.Identifier);
			Assert.IsTrue(lazyInitializer.IsUninitialized);

			Assert.AreEqual(uste1.Id, rev1.Reference.Id);
			Assert.AreEqual(uste1.Str, rev1.Reference.Str);
			Assert.IsFalse(lazyInitializer.IsUninitialized);
		}

		[Test]
		public void VerifyNullReferenceWithNotFoundActionIgnore()
		{
			var mtoRev2 = AuditReader().Find<ManyToOneNotAuditedNullEntity>(mtonane1.Id, 2);
			mtoRev2.Should().Be.EqualTo(mtonane1);
			mtoRev2.Reference.Should().Be.Null();

			var mtmRev2 = AuditReader().Find<ManyToManyNotAuditedNullEntity>(mtmnane1.Id, 2);
			mtmRev2.Should().Be.EqualTo(mtmnane1);
			mtmRev2.References.Should().Be.Empty();

			var otmRev2 = AuditReader().Find<OneToManyNotAuditedNullEntity>(otmnane1.Id, 2);
			otmRev2.Should().Be.EqualTo(otmnane1);
			otmRev2.References.Should().Be.Empty();
		}
	}
}