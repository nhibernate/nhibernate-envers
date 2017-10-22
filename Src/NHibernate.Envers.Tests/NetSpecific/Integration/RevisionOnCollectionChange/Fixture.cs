using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.RevisionOnCollectionChange
{
	[TestFixture]
	public class Fixture : TestBase
	{
		private object _newOwnerId;
		private object _carId;

		public Fixture(string strategyType) : base(strategyType)
		{
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.GlobalWithModifiedFlag, true);
		}

		protected override void Initialize()
		{
			using (var tx = Session.BeginTransaction())
			{
				var oldOwner = new Person();
				Session.Save(oldOwner);
				var newOwner = new Person();
				_newOwnerId = Session.Save(newOwner);
				var car = new Car { Owner = oldOwner };
				_carId = Session.Save(car);
				tx.Commit();
			}
		}

		[Test]
		public void ChangeParentReferenceShouldNotThwrowException()
		{
			using (var tx = Session.BeginTransaction())
			{
				var car = Session.Get<Car>(_carId);
				car.Owner = Session.Get<Person>(_newOwnerId);
				tx.Commit();
			}
		}
	}
}