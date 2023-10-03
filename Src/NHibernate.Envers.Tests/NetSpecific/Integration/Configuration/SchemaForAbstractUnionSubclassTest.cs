using System.Collections.Generic;
using NHibernate.Envers.Tests.Integration.Interfaces.Inheritance.PropertiesAudited;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Configuration
{
	public partial class SchemaForAbstractUnionSubclassTest : TestBase
	{
		private long id;

		public SchemaForAbstractUnionSubclassTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var ai = new AuditedImplementor { Data = "La data", AuditedImplementorData = "audited implementor data" };
			using (var tx = Session.BeginTransaction())
			{
				id = (long) Session.Save(ai);
				tx.Commit();
			}
		}

		[Test]
		public void ShouldBeAbleToQueryBaseType()
		{
			AuditReader().Find<ISimple>(id, 1)
				.Should().Not.Be.Null();
		}

		[Test]
		public void ShouldNotCreateTable()
		{
			Cfg.GetClassMapping("NHibernate.Envers.Tests.Integration.Interfaces.Inheritance.PropertiesAudited.ISimple_AUD")
				.Table.IsPhysicalTable
				.Should().Be.False();
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] {"Integration.Interfaces.Inheritance.PropertiesAudited.Union.Mapping.hbm.xml"}; }
		}
	}
}