using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.RevEntity
{
	[TestFixture]
	public class CustomColumnInheritanceTest : TestBase
	{
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
			const string auditName = TestAssembly + ".Integration.Inheritance.Entities.ChildEntity_AUD";
			var columns = Cfg.GetClassMapping(auditName).Key.ColumnIterator;
			Assert.AreEqual("int", ((Column)columns.Last()).SqlType);
		}
	}
}