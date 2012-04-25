using System.Linq;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Fluent;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.OneToMany.Detached;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Fluent
{
	[TestFixture]
	public class TableNameTest
	{
		[Test]
		public void ShouldSetTableInfoGeneric()
		{
			var fluentCfg = new FluentConfiguration();
			fluentCfg.Audit<StrTestEntity>()
				.SetTableInfo(table =>
								{
									table.Schema = "testschema";
									table.Catalog = "testcatalog";
									table.Value = "tableName";
								});
			CheckTableInfo(fluentCfg);
		}

		[Test]
		public void ShouldSetTableInfo()
		{
			var fluentCfg = new FluentConfiguration();
			fluentCfg.Audit(typeof(StrTestEntity))
				.SetTableInfo(table =>
				{
					table.Schema = "testschema";
					table.Catalog = "testcatalog";
					table.Value = "tableName";
				});
			CheckTableInfo(fluentCfg);
		}

		[Test]
		public void ShouldSetJoinTableInfoGeneric()
		{
			var fluentCfg = new FluentConfiguration();
			fluentCfg.Audit<SetRefCollEntity>()
				.SetTableInfo(r => r.Collection,
									table =>
									{
										table.Schema = "testschema";
										table.Catalog = "testcatalog";
										table.TableName = "tableName";
										table.InverseJoinColumns = new[] { "donald", "duck" };
									});
			CheckJoinTableInfo(fluentCfg);
		}

		[Test]
		public void ShouldSetJoinTableInfoGenericUsingString()
		{
			var fluentCfg = new FluentConfiguration();
			fluentCfg.Audit<SetRefCollEntity>()
				.SetTableInfo("Collection",
									table =>
									{
										table.Schema = "testschema";
										table.Catalog = "testcatalog";
										table.TableName = "tableName";
										table.InverseJoinColumns = new[] { "donald", "duck" };
									});
			CheckJoinTableInfo(fluentCfg);
		}

		[Test]
		public void ShouldSetJoinTableInfo()
		{
			var fluentCfg = new FluentConfiguration();
			fluentCfg.Audit(typeof(SetRefCollEntity))
				.SetTableInfo("Collection", 
									table =>
							   {
								  table.Schema = "testschema";
								  table.Catalog = "testcatalog";
								  table.TableName = "tableName";
								table.InverseJoinColumns = new[] {"donald", "duck"};
							   });
			CheckJoinTableInfo(fluentCfg);	
		}

		private static void CheckTableInfo(IMetaDataProvider fluentCfg)
		{
			var metas = fluentCfg.CreateMetaData(null);
			var entMeta = metas[typeof(StrTestEntity)];

			var result = entMeta.ClassMetas.OfType<AuditTableAttribute>().Single();

			result.Catalog.Should().Be.EqualTo("testcatalog");
			result.Schema.Should().Be.EqualTo("testschema");
			result.Value.Should().Be.EqualTo("tableName");
		}

		private static void CheckJoinTableInfo(IMetaDataProvider fluentCfg)
		{
			var metas = fluentCfg.CreateMetaData(null);
			var entMeta = metas[typeof(SetRefCollEntity)];

			var result = entMeta.MemberMetas[typeof(SetRefCollEntity).GetProperty("Collection")]
				.OfType<AuditJoinTableAttribute>().Single();

			result.Catalog.Should().Be.EqualTo("testcatalog");
			result.Schema.Should().Be.EqualTo("testschema");
			result.TableName.Should().Be.EqualTo("tableName");
			result.InverseJoinColumns.Should().Have.SameSequenceAs("donald", "duck");
		}
	}
}