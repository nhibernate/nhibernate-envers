using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Mapping;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.SuperClass.AuditParents
{
	/// <summary>
	/// Tests mapping of child entity which parent declares one of its ancestors as audited with <see cref="AuditedAttribute.AuditParents"/>
	/// property. Child entity may mark explicitly its parent as audited or not.
	/// </summary>
	[TestFixture]
	public class TransitiveAuditParentsTest : TestBase
	{
		private const long childImpTransId = 17;
		private const long childExpTransId = 23;

		protected override void Initialize()
		{
			//revision 1
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(new ImplicitTransitiveChildEntity
				             	{
				             		Child = "child 1",
				             		GrandParent = "grandparent 1",
				             		Id = childImpTransId,
				             		NotAudited = "notaudited 1",
				             		Parent = "parent 1"
				             	});
				tx.Commit();
			}

			//revision 2
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(new ExplicitTransitiveChildEntity
				{
					Child = "child 2",
					GrandParent = "grandparent 2",
					Id = childExpTransId,
					NotAudited = "notaudited 2",
					Parent = "parent 2"
				});
				tx.Commit();
			}
		}

		[Test]
		public void VerifyCreatedAuditTables()
		{
			var explicitTable = Cfg.GetClassMapping(typeof(ExplicitTransitiveChildEntity).FullName + "_AUD").Table;
			var implicitTable = Cfg.GetClassMapping(typeof(ImplicitTransitiveChildEntity).FullName + "_AUD").Table;

			checkTableColumns(new[] { "child", "parent", "grandparent", "id" }, new[] { "notAudited" }, explicitTable);
			checkTableColumns(new[] { "child", "parent", "grandparent", "id" }, new[] { "notAudited" }, implicitTable);
		}

		private void checkTableColumns(IEnumerable<string> expectedColumns, IEnumerable<string> unexpectedColumns, Table table)
		{
			foreach (var columnName in expectedColumns)
			{
				table.GetColumn(new Column(columnName))
					.Should().Not.Be.Null();
			}

			foreach (var columnName in unexpectedColumns)
			{
				table.GetColumn(new Column(columnName))
					.Should().Be.Null();
			}
		}

		[Test]
		public void VerifyImplicitTransitiveAuditParents()
		{
			// expectedChild.notAudited shall be null, because it is not audited.
			var expectedChild = new ImplicitTransitiveChildEntity
			                    	{
			                    		Child = "child 1",
			                    		GrandParent = "grandparent 1",
			                    		Id = childImpTransId,
			                    		NotAudited = null,
			                    		Parent = "parent 1"
			                    	};
			var child = AuditReader().Find<ImplicitTransitiveChildEntity>(childImpTransId, 1);
			child.Should().Be.EqualTo(expectedChild);
		}

		[Test]
		public void VerifyExplicitTransitiveAuditParents()
		{
			// expectedChild.notAudited shall be null, because it is not audited.
			var expectedChild = new ExplicitTransitiveChildEntity
			{
				Child = "child 2",
				GrandParent = "grandparent 2",
				Id = childExpTransId,
				NotAudited = null,
				Parent = "parent 2"
			};
			var child = AuditReader().Find<ExplicitTransitiveChildEntity>(childExpTransId, 2);
			child.Should().Be.EqualTo(expectedChild);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Integration.SuperClass.AuditParents.Mapping.hbm.xml", "Entities.Mapping.hbm.xml" };
			}
		}
	}
}