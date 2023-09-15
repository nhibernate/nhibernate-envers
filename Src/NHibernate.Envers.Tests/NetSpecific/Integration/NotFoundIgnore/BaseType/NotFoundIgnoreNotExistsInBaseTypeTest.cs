using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.NotFoundIgnore.BaseType
{
	public partial class NotFoundIgnoreNotExistsInBaseTypeTest : TestBase
	{
		private Guid parentId;

		public NotFoundIgnoreNotExistsInBaseTypeTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var childName = new ChildName { Name = "the child" };
			var parent = new Parent();
			var girl = new Girl { Name = childName };
			parent.Children = new List<Child> { girl };
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(childName);
				parentId = (Guid)Session.Save(parent);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(childName);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			AuditReader().GetRevisions(typeof(Parent), parentId)
				.Should().Have.SameSequenceAs(1);
		}

		[Test]
		public void VerifyHistory()
		{
			AuditReader().Find<Parent>(parentId, 1).Children
				.Single().Name.Should().Be.Null();
		}
	}
}