using NHibernate.Cfg;
using NHibernate.Envers.Configuration.Fluent;
using NHibernate.Envers.Event;
using NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent.Model;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent
{
	[TestFixture]
	public class NonMappedRevisionEntityTest
	{
		[Test]
		public void ShouldThrow()
		{
			var cfg = new Cfg.Configuration();
			var fluent = new FluentConfiguration();
			fluent.SetRevisionEntity<RevisionEntity>(rev => rev.Number, rev => rev.Timestamp);
			cfg.Executing(x => x.IntegrateWithEnvers(new AuditEventListener(), fluent))
				.Throws<FluentException>();
		}
	}
}