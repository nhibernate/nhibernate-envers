using NHibernate.Envers.Exceptions;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.AuditReader
{
	[TestFixture]
	public class AuditReaderAPITest : TestBase
	{
		protected override void Initialize()
		{
			var ent1 = new AuditedTestEntity {Data = "str1"};
			var ent2 = new NotAuditedTestEntity {Data = "str1"};
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(ent1);
				Session.Save(ent2);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				ent1.Data = "str2";
				ent2.Data = "str2";
				tx.Commit();
			}
		}

		[Test]
		public void ShouldAudit()
		{
			AuditReader().IsEntityClassAudited(typeof (AuditedTestEntity))
				.Should().Be.True();
			AuditReader().IsEntityNameAudited(typeof(AuditedTestEntity).FullName)
				.Should().Be.True();
			foreach (var revision in AuditReader().GetRevisions<AuditedTestEntity>(1))
			{
				var foo = revision;
			}

			AuditReader().GetRevisions<AuditedTestEntity>(1)
				.Should().Have.SameSequenceAs(1, 2);
		}

		[Test]
		public void ShouldNotAudit()
		{
			AuditReader().IsEntityClassAudited(typeof(NotAuditedTestEntity))
				.Should().Be.False();
			AuditReader().IsEntityNameAudited(typeof(NotAuditedTestEntity).FullName)
				.Should().Be.False();

			Assert.Throws<NotAuditedException>(() =>
			   AuditReader().GetRevisions<NotAuditedTestEntity>(1));
		}
	}
}