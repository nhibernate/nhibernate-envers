using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Flush
{
	[TestFixture]
	public class DoubleFlushAddDelTest : TestBase
	{
		private int id1;

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Mapping.hbm.xml" }; }
		}

		protected override FlushMode FlushMode
		{
			get
			{
				return FlushMode.Never;
			}
		}

		protected override void Initialize()
		{
			using (var tx = Session.BeginTransaction())
			{
				var fe = new StrTestEntity {Str = "x"};
				id1 = (int)Session.Save(fe);
				Session.Flush();
				Session.Delete(fe);
				Session.Flush();
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.IsEmpty(AuditReader().GetRevisions(typeof(StrTestEntity), id1));
		}

		[Test]
		public void VerifyHistoryOfId1()
		{
			Assert.IsNull(AuditReader().Find<StrTestEntity>(id1, 1));
		}
	}
}