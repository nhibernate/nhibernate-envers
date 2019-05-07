using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.ComponentAsId
{
	public class ComponentAsIdTest : TestBase
	{
		public ComponentAsIdTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}
		
		[Test]
		public void ComponentAsIdTestMethod()
		{
			Assert.DoesNotThrow(() =>
			{
				var ent1 = new Entity1
				{
					Id = 1
				};

				Save(ent1);

				var ent2 = new Entity2()
				{
					Id = 1
				};

				Save(ent2);

				var udf = new SomeEntUDF
				{
					Id = new ComponentAsId
					{
						Key1 = ent1,
						Key2 = ent2
					}
				};

				Save(udf);

				Del(udf);
				Del(ent1);
			});
		}

		void Save(object o)
		{
			using (var tran = Session.BeginTransaction())
			{
				Session.Save(o);
				tran.Commit();
			}
		}

		void Del(object o)
		{
			using (var tran = Session.BeginTransaction())
			{
				Session.Delete(o);
				tran.Commit();
			}
		}

		protected override void Initialize()
		{
		}
	}
}
