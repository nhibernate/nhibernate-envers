using System.Reflection;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Event;
using NUnit.Framework;
using NHibernate.Cfg;

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
				.AddResource(TestBase.TestAssembly + ".Entities.RevEntity.CustomRevEntity.hbm.xml", currAss)
				.AddResource(TestBase.TestAssembly + ".Entities.RevEntity.CustomDateRevEntity.hbm.xml", currAss)
				.IntegrateWithEnvers(new AuditEventListener(), new AttributeMetaDataProvider());

			Assert.Throws<MappingException>(() => cfg.BuildSessionFactory());
		}
	}
}