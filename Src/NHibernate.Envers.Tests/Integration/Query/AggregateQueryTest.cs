using System;
using System.Collections.Generic;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Query
{
    [TestFixture]
    public class AggregateQueryTest : TestBase
    {
        protected override void Initialize()
        {
            var ite1 = new IntTestEntity {Number = 2};
            var ite2 = new IntTestEntity { Number = 10};
            var ite3 = new IntTestEntity { Number = 8};

            using(var tx = Session.BeginTransaction())
            {
                Session.Save(ite1);
                Session.Save(ite2);
                tx.Commit();
            }
            using(var tx = Session.BeginTransaction())
            {
                Session.Save(ite3);
                ite1.Number = 0;
                tx.Commit();
            }
            using (var tx = Session.BeginTransaction())
            {
                ite2.Number = 52;
                tx.Commit();
            }
        }

        [Test]
        public void VerifyEntitiesAvgMaxQuery()
        {
            var ver1 = (object[])AuditReader.CreateQuery()
                .ForEntitiesAtRevision(typeof (IntTestEntity), 1)
                .AddProjection(AuditEntity.Property("Number").Max())
                .AddProjection(AuditEntity.Property("Number").Function("avg"))
                .GetSingleResult();
            var ver2 = (object[])AuditReader.CreateQuery()
                .ForEntitiesAtRevision(typeof(IntTestEntity), 2)
                .AddProjection(AuditEntity.Property("Number").Max())
                .AddProjection(AuditEntity.Property("Number").Function("avg"))
                .GetSingleResult();
            var ver3 = (object[])AuditReader.CreateQuery()
                .ForEntitiesAtRevision(typeof(IntTestEntity), 3)
                .AddProjection(AuditEntity.Property("Number").Max())
                .AddProjection(AuditEntity.Property("Number").Function("avg"))
                .GetSingleResult();

            Assert.AreEqual(10, ver1[0]);
            Assert.AreEqual(6, ver1[1]);

            Assert.AreEqual(10, ver2[0]);
            Assert.AreEqual(6, ver2[1]);

            Assert.AreEqual(52, ver3[0]);
            Assert.AreEqual(20, ver3[1]);
        }

        protected override IEnumerable<string> Mappings
        {
            get
            {
                return new[]{"Entities.Mapping.hbm.xml"};
            }
        }
    }
}