using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Event;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.RevInfo
{
	[TestFixture]
	public class MultipleRevInfoTest
	{
		[Test]
		public void MultipleRevisionEntitiesShouldThrow()
		{
			var currAss = Assembly.GetExecutingAssembly();
			
			var cfg = new Cfg.Configuration()
				.Configure()
				.AddResource("NHibernate.Envers.Tests.Entities.RevEntity.CustomRevEntity.hbm.xml", currAss)
				.AddResource("NHibernate.Envers.Tests.Entities.RevEntity.CustomDateRevEntity.hbm.xml", currAss);

			cfg.Executing(x => x.IntegrateWithEnvers(new AuditEventListener(), new AttributeConfiguration())).Throws<MappingException>();
		}
	}
}