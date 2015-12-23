using System.Linq;
using NHibernate.Envers.Query;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Access.None.ManyToOne
{
	[Ignore("Failing tests for NHE-145")]
	public class Fixture : TestBase
	{
		private int parentId;

		public Fixture(string strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var parent = new Parent();
			parent.AddChild("child1");
			using (var tx = Session.BeginTransaction())
			{
				parentId = (int) Session.Save(parent);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				parent.AddChild("child2");
				tx.Commit();
			}
		}

		[Test]
		public void CanQueryOnAccessNoneProperty()
		{
			AuditReader().CreateQuery()
				.ForHistoryOf<Child, DefaultRevisionEntity>()
				.Add(AuditEntity.Property("ParentId").Eq(parentId))
				.Results()
				.Count().Should().Be.EqualTo(2);
		}

		[Test]
		public void CanQueryOnAccessNoneManyToOne()
		{
			var parent = new Parent {Id = parentId};
			AuditReader().CreateQuery()
				.ForHistoryOf<Child, DefaultRevisionEntity>()
				.Add(AuditEntity.Property("Parent2").Eq(parent))
				.Results()
				.Count().Should().Be.EqualTo(2);
		}
	}
}