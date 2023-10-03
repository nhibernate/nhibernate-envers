using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.NotFoundIgnore.BaseType
{
	public partial class NotFoundIgnoreExistsInBaseTypeTest : TestBase
	{
		private Guid parentId;

		public NotFoundIgnoreExistsInBaseTypeTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var childName = new ChildName {Name = "the child"};
			var parent = new Parent();
			var boy = new Boy();
			var girl = new Girl{Name = childName};
			parent.Children = new List<Child> {boy};
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(childName);
				parentId = (Guid) Session.Save(parent);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				parent.Children.Add(girl);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			AuditReader().GetRevisions(typeof(Parent), parentId)
				.Should().Have.SameSequenceAs(1, 2);
		}

		[Test]
		public void VerifyHistory()
		{
			var ver1children = AuditReader().Find<Parent>(parentId, 1).Children;
			var ver2children = AuditReader().Find<Parent>(parentId, 2).Children;

			ver1children.Single().Sex.Should().Be.EqualTo("Boy");
			ver2children[0].Sex.Should().Be.EqualTo("Boy");
			ver2children[1].Sex.Should().Be.EqualTo("Girl");
		}
	}
}