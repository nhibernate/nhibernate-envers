using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Mapping;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.SuperClass.AuditParents
{
	/// <summary>
	/// Tests mapping of child entity that declares all of its ancestors as audited with <see cref="AuditedAttribute.AuditParents"/> property.
	/// </summary>
	[TestFixture]
	public class MultipleAuditParentsTest : TestBase
	{
		private const long childMultipleId = 11;
		private int siteMultipleId;

		protected override void Initialize()
		{
			var siteMultiple = new StrIntTestEntity {Str = "data 1", Number = 1};

			//Revision 1
			using (var tx = Session.BeginTransaction())
			{
				siteMultipleId = (int) Session.Save(siteMultiple);
				Session.Save(new ChildMultipleParentsEntity
				             	{
				             		Id = childMultipleId,
				             		Relation = siteMultiple,
				             		NotAudited = "not audited 1",
				             		Child = "child 1",
				             		GrandParent = "grandparent 1",
				             		Parent = "parent 1"
				             	});
				tx.Commit();
			}
		}

		[Test]
		public void VerifyCreatedAuditTable()
		{
			var expectedColumns = new HashSet<string> {"Child", "Parent", "Relation", "GrandParent", "Id"};
			var unexpectedColumns = new HashSet<string> {"NotAudited"};

			var table = Cfg.GetClassMapping(typeof (ChildMultipleParentsEntity).FullName + "_AUD").Table;

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
		public void VerifyMultipleAuditParents()
		{
			// expectedMultipleChild.notAudited shall be null, because it is not audited.
			var expectedMultipleChild = new ChildMultipleParentsEntity
													{
														Id = childMultipleId,
														Relation = new StrIntTestEntity { Str = "data 1", Number = 1, Id = siteMultipleId },
														NotAudited = null,
														Child = "child 1",
														GrandParent = "grandparent 1",
														Parent = "parent 1"
													};
			var child = AuditReader().Find<ChildMultipleParentsEntity>(childMultipleId, 1);
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