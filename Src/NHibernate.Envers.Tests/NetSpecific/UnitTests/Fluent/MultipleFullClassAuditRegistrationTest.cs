using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Fluent;
using NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent.Model;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent
{
	[TestFixture]
	public class MultipleFullClassAuditRegistrationTest
	{
		[Test]
		public void WhenRegisterTypeThenIsAudited()
		{
			var cfg = new FluentConfiguration();
			cfg.Audit(new[] { typeof(Animal) });
			cfg.CreateMetaData(null).Keys.Should().Contain(typeof(Animal)).And.Have.Count.EqualTo(1);
		}

		[Test]
		public void WhenRegisterInheritedThenBaseIsAudited()
		{
			var cfg = new FluentConfiguration();
			cfg.Audit(new[] { typeof(Dog), typeof(Cat) });
			cfg.CreateMetaData(null).Keys.Should().Have.SameValuesAs(new[] { typeof(Animal), typeof(Cat), typeof(Dog) });
		}

		[Test]
		public void WhenRegisterInheritedThenAllHasOnlyAuditedAttribute()
		{
			var cfg = new FluentConfiguration();
			cfg.Audit(new[] { typeof(Dog), typeof(Cat) });
			var metas = cfg.CreateMetaData(null);
			foreach (var entityMeta in metas.Values)
			{
				entityMeta.ClassMetas.OnlyContains<AuditedAttribute>();
			}
		}
	}
}