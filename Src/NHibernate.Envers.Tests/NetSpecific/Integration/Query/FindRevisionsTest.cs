

using System.Collections.Generic;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Query
{
    [TestFixture]
    public class FindRevisionsTest : TestBase
    {
        private int id = 13;

        protected override void Initialize()
        {
            var person = new Person { Id = id };

            using (var tx = Session.BeginTransaction())
            {
                Session.Save(person);
                tx.Commit();
            }
            using (var tx = Session.BeginTransaction())
            {
                person.Weight.Kilo = 34;
                tx.Commit();
            }
        }

        [Test]
        public void ShouldReturnOne()
        {
            var rev = new List<long> { 6, 3, 11, 15, 26, 1 };
            var revs = AuditReader().FindRevisions(rev);

            revs.Should().Have.Count.EqualTo(1);
            revs[1].Should().Be.InstanceOf<DefaultRevisionEntity>();
        }

        [Test]
        public void ShouldReturnAll()
        {
            var rev = new List<long> { 1, 2 };
            var revs = AuditReader().FindRevisions<DefaultRevisionEntity>(rev);

            revs.Should().Have.Count.EqualTo(2);
            revs[1].Should().Be.InstanceOf<DefaultRevisionEntity>();
            revs[1].Id.Should().Be.EqualTo(1);
        }
    }
}