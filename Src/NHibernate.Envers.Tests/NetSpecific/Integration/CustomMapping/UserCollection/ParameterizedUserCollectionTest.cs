using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.CustomMapping.UserCollection
{
	public partial class ParameterizedUserCollectionTest : TestBase
	{
		private const int id = 123;

		public ParameterizedUserCollectionTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var entity = new Entity {Id = 123};
			entity.SpecialCollection.Add(new Number{Value = 7});
			entity.SpecialCollection.Add(new Number{Value = 13});
			entity.SpecialCollection.Add(new Number{Value = 5});
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(entity);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				entity.SpecialCollection.Add(new Number { Value = 123 });
				entity.SpecialCollection.Add(new Number { Value = 200 });
				tx.Commit();
			}
		}

		[Test]
		public void VerifyNhCoreMapping()
		{
			var entity = Session.Get<Entity>(id);
			entity.SpecialCollection.ItemsOverLimit()
				.Should().Be.EqualTo(3);
		}

		[Test]
		public void VerifyRevision1()
		{
			var rev1 = AuditReader().Find<Entity>(id, 1);
			rev1.SpecialCollection.ItemsOverLimit()
				.Should().Be.EqualTo(1);
		}

		[Test]
		public void VerifyRevision2()
		{
			var rev1 = AuditReader().Find<Entity>(id, 2);
			rev1.SpecialCollection.ItemsOverLimit()
				.Should().Be.EqualTo(3);
		}
	}
}