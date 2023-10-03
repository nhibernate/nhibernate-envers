using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Strategy.SetOfValues
{
	public partial class SetOfValuesTest : ValidityTestBase
	{
		private int id;

		protected override void Initialize()
		{
			var c = new SetOfValuesTestEntity();
			c.ChildValues.Add("v");

			using (var tx = Session.BeginTransaction())
			{
				id = (int)Session.Save(c);
				tx.Commit();
			}

			using (var tx = Session.BeginTransaction())
			{
				c.ChildValues.Remove("v");
				tx.Commit();
			}

			// Adds to the collection a value that is equal to one that has been removed before
			using (var tx = Session.BeginTransaction())
			{
				c.ChildValues.Add("v");
				tx.Commit();
			}
		}

		[Test]
		public void SameValueCanBeAddedToAndThenRemovedFromTheColectionMoreThanOnce()
		{
			// Remove value that has been removed and then added back before
			using (var tx = Session.BeginTransaction())
			{
				var c = Session.Get<SetOfValuesTestEntity>(id);
				c.ChildValues.Remove("v");
				tx.Commit();
			}
			Assert.Pass("Value successfuly removed");
		}
	}
}