using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Mapping;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.SuperClass.AuditParents
{
	/// <summary>
	/// Tests mapping of baby entity which declares its parent as audited with <see cref="AuditedAttribute.AuditParents"/> property.
	/// Moreover, child class (mapped superclass of baby entity) declares grandparent entity as audited. In this case all 
	/// attributes of baby class shall be audited.
	/// </summary>
	[TestFixture]
	public class TotalAuditParentsTest : TestBase
	{
		private const long babyCompleteId = 13;
		private int siteCompleteId;

		protected override void Initialize()
		{
			var siteMultiple = new StrIntTestEntity { Str = "data 1", Number = 1 };

			//Revision 1
			using (var tx = Session.BeginTransaction())
			{
				siteCompleteId = (int) Session.Save(siteMultiple);
				Session.Save(new BabyCompleteEntity()
				{
					Id = babyCompleteId,
					Relation = siteMultiple,
					NotAudited = "not audited 1",
					Child = "child 1",
					GrandParent = "grandparent 1",
					Parent = "parent 1",
					Baby="baby 1"
				});
				tx.Commit();
			}
		}

		[Test]
		public void VerifyCreatedAuditTable()
		{
			var expectedColumns = new HashSet<string> { "Baby", "Child", "Parent", "Relation", "GrandParent", "Id" };
			var unexpectedColumns = new HashSet<string> { "NotAudited" };

			var table = Cfg.GetClassMapping(typeof(BabyCompleteEntity).FullName + "_AUD").Table;

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
		public void VerifyCompleteAuditParents()
		{
			// expectedBaby.notAudited shall be null, because it is not audited.
			var expectedMultipleChild = new BabyCompleteEntity()
			{
				Id = babyCompleteId,
				Relation = new StrIntTestEntity { Str = "data 1", Number = 1, Id = siteCompleteId },
				NotAudited = null,
				Child = "child 1",
				GrandParent = "grandparent 1",
				Parent = "parent 1",
				Baby = "baby 1"
			};
			var child = AuditReader().Find<BabyCompleteEntity>(babyCompleteId, 1);
			child.Should().Be.EqualTo(expectedMultipleChild);
			child.Relation.Should().Be.EqualTo(expectedMultipleChild.Relation);
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