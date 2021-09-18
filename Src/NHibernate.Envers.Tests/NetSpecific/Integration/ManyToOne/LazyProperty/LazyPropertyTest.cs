using System.Linq;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.ManyToOne.LazyProperty
{
	public class LazyPropertyTest : TestBase
	{
		private long id_pers1;

		public LazyPropertyTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var pers1 = new Person {Name = "Hernan"};

			using (var tx = Session.BeginTransaction())
			{
				id_pers1 = (long) Session.Save(pers1);
				tx.Commit();
			}
		}

		[Test]
		public void SavePersonProxyForFieldInterceptor()
		{
			using (var tx = Session.BeginTransaction())
			{
				var pers = Session.Query<Person>().Single(x => x.Id == id_pers1);
				var car = new Car
				{
					Owner = pers
				};
				id_pers1 = (long) Session.Save(car);
				tx.Commit();
			}
		}
	}
}