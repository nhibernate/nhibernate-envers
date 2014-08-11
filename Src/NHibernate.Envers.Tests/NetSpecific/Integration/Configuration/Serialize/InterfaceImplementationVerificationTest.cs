using System.Linq;
using NHibernate.Envers.Configuration.Metadata;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Id;
using NHibernate.Envers.Entities.Mapper.Relation;
using NHibernate.Envers.RevisionInfo;
using NHibernate.Envers.Strategy;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Configuration.Serialize
{
	[TestFixture]
	public class InterfaceImplementationVerificationTest
	{
		private readonly System.Type[] serializableInterfaces =
		{
			typeof (ICollectionMapperFactory),
			typeof (IEnversProxyFactory),
			typeof (IRevisionInfoGenerator),
			typeof (IPropertyMapper),
			typeof (IIdMapper),
			typeof (IAuditStrategy)
		};

		[Test]
		public void ShouldImplementSerializable()
		{
			foreach (var enversType in typeof(IAuditReader).Assembly.GetTypes()
						.Where(enversType => enversType.IsClass && 
											serializableInterfaces.Any(serializableInterface => serializableInterface.IsAssignableFrom(enversType))))
			{
				Assert.IsTrue(enversType.IsSerializable, enversType + " must implement serializable.");
			}
		}
	}
}