using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.ManyToOne.UniDirectional;
using NHibernate.Proxy;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Proxy
{
    [TestFixture]
    public class ProxyIdentifierTest : TestBase
    {
        private TargetNotAuditedEntity tnae1;
        private UnversionedStrTestEntity uste1;

        protected override IEnumerable<string> Mappings
        {
            get
            {
                return new[] { "Entities.Mapping.hbm.xml", "Entities.ManyToOne.UniDirectional.Mapping.hbm.xml" };
            }
        }

        protected override void Initialize()
        {
            uste1 = new UnversionedStrTestEntity { Str = "str1" };

            // No revision
            using(var tx =Session.BeginTransaction())
            {
                Session.Save(uste1);
                tx.Commit();
            }

            // Revision 1
            using(var tx =Session.BeginTransaction())
            {
                tnae1 = new TargetNotAuditedEntity {Id = 1, Data = "tnae1", Reference = uste1};
                Session.Save(tnae1);
                tx.Commit();
            }
        }

        [Test]
        public void VerifyProxyIdentifier()
        {
            var rev1 = AuditReader.Find<TargetNotAuditedEntity>(tnae1.Id, 1);
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
    }
}