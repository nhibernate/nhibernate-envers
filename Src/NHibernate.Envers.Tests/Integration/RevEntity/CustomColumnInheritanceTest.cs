using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.RevEntity
{
	public class CustomColumnInheritanceTest : TestBase
	{
		public CustomColumnInheritanceTest(string strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Integration.Inheritance.Joined.Mapping.hbm.xml", 
									"Entities.RevEntity.CustomRevEntityColumnMapping.hbm.xml" };
			}
		}

		protected override void Initialize()
		{
		}

		[Test]
		public void ChildRevColumnTypeShouldBeOfIntType()
		{
			var auditName = TestAssembly + ".Integration.Inheritance.Entities.ChildEntity_AUD";
			var columns = Cfg.GetClassMapping(auditName).Key.ColumnIterator;
			Assert.AreEqual("int", ((Column)columns.Last()).SqlType);
		}
	}
}