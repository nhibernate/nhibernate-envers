using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.AccessType
{
    [TestFixture]
    public class FieldAccessTest : TestBase
    {
        private int id;

        protected override void Initialize()
        {
            var fa = new FieldAccessEntity {Data = "first"};
            using(var tx = Session.BeginTransaction())
            {
                id = (int)Session.Save(fa);
                tx.Commit();
            }
            using(var tx = Session.BeginTransaction())
            {
                fa.Data = "second";
                tx.Commit();
            }
        }
        
        [Test]
        public void VerifyRevisionCount()
        {
            CollectionAssert.AreEquivalent(new[] {1, 2}, AuditReader.GetRevisions(typeof (FieldAccessEntity), id));
        }

        [Test]
        public void VerifyHistory()
        {
            var ver1 = new FieldAccessEntity { Id = id, Data = "first" };
            var ver2 = new FieldAccessEntity {Id = id, Data = "second"};

            Assert.AreEqual(ver1, AuditReader.Find<FieldAccessEntity>(id, 1));
            Assert.AreEqual(ver2, AuditReader.Find<FieldAccessEntity>(id, 2));
        }
    }
}