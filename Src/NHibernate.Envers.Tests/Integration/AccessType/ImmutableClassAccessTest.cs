using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.AccessType
{
	public partial class ImmutableClassAccessTest : TestBase
	{
		private Country country;

		public ImmutableClassAccessTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			using(var tx = Session.BeginTransaction())
			{
				country = new Country(123, "Germany");
				Session.Save(country);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1 }, AuditReader().GetRevisions(typeof(Country), country.Code));
		}

		[Test]
		public void VerifyHistory()
		{
			var country1 = Session.Get<Country>(country.Code);
			Assert.AreEqual(country, country1);

			var history = AuditReader().Find<Country>(country1.Code, 1);
			Assert.AreEqual(country, history);
		}
	}
}