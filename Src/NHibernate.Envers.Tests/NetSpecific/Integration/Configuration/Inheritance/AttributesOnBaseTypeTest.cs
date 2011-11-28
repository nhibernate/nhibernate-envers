using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Configuration.Inheritance
{
	[TestFixture]
	public class AttributesOnBaseTypeTest 
	{
		private IDictionary<System.Type, IEntityMeta> metaData;

		[SetUp]
		public void Setup()
		{
			var cfg = new Cfg.Configuration();
			cfg.Configure();
			var ass = GetType().Assembly;
			cfg.AddResource("NHibernate.Envers.Tests.NetSpecific.Integration.Configuration.Inheritance.Mapping.hbm.xml", ass);
			var attrConf = new AttributeConfiguration();

			metaData = attrConf.CreateMetaData(cfg);
		}

		[Test]
		public void ShouldFindMemberAttributeInBaseClass()
		{
			var member = typeof(Super).GetProperty("Number");
			metaData[typeof (Super)].MemberMetas[member].OnlyContains<NotAuditedAttribute>();
		}

		[Test]
		public void ShouldFindClassAttributeInBaseClass()
		{
			metaData[typeof(Base)].ClassMetas.OnlyContains<AuditedAttribute>();
		}
	}
}