using System.Collections.Generic;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.CustomMapping.ImplicitEnversCollectionType
{
	public partial class Fixture : TestBase
	{
		private int parentId;

		public Fixture(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var parent = new Parent {Children = new List<Child> {new Child()}};

			using (var tx = Session.BeginTransaction())
			{
				parentId = (int) Session.Save(parent);
				tx.Commit();
			}

			using (var tx = Session.BeginTransaction())
			{
				parent.Children.Add(new Child());
				tx.Commit();
			}
		}

		[Test]
		public void VerifyNhCoreMapping()
		{
			var entity = Session.Get<Parent>(parentId);
			entity.Children.Count
				.Should().Be.EqualTo(2);
		}

		[Test]
		public void VerifyRevision1()
		{
			var entity = AuditReader().Find<Parent>(parentId, 1);
			entity.Children.Count
				.Should().Be.EqualTo(1);
		}

		[Test]
		public void VerifyRevision2()
		{
			var entity = AuditReader().Find<Parent>(parentId, 2);
			entity.Children.Count
				.Should().Be.EqualTo(2);
		}
	}
}