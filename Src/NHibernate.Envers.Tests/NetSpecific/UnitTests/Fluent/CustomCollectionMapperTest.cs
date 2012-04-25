using System.Linq;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Fluent;
using NHibernate.Envers.Tests.Entities.OneToMany.Detached;
using NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent.Model;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent
{
	[TestFixture]
	public class CustomCollectionMapperTest
	{
		[Test]
		public void ShouldSetMapperGeneric()
		{
			var propInfo = typeof (SetRefCollEntity).GetProperty("Collection");
			var fluentCfg = new FluentConfiguration();
			fluentCfg.Audit<SetRefCollEntity>()
				.SetCollectionMapper<SomeCollectionMapper>(e => e.Collection);

			var metas = fluentCfg.CreateMetaData(null);
			var memberMetas = metas[typeof (SetRefCollEntity)].MemberMetas[propInfo];
			memberMetas.OnlyContains<CustomCollectionMapperAttribute>();
			memberMetas.OfType<CustomCollectionMapperAttribute>().First().CustomCollectionFactory
				.Should().Be.EqualTo<SomeCollectionMapper>();
		}
	}
}