using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Flush
{
	public partial class DoubleFlushModModTest :TestBase
	{
		private int id;

		public DoubleFlushModModTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings => new[] { "Entities.Mapping.hbm.xml" };

		protected override FlushMode FlushMode => FlushMode.Manual;

		protected override void Initialize()
		{
			var fe = new StrTestEntity { Str = "x" };
			using (var tx = Session.BeginTransaction())
			{
				id = (int) Session.Save(fe);
				Session.Flush();
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				fe.Str = "y";
				Session.Flush();
				fe.Str = "z";
				Session.Flush();
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(StrTestEntity), id));
		}

		[Test]
		public void VerifyHistoryOfId()
		{
			var ver1 = new StrTestEntity { Id = id, Str = "x"};
			var ver2 = new StrTestEntity { Id = id, Str = "z" };

			Assert.AreEqual(ver1, AuditReader().Find<StrTestEntity>(id, 1));
			Assert.AreEqual(ver2, AuditReader().Find<StrTestEntity>(id, 2));
		}

		[Test]
		public void VerifyRevisionTypes()
		{
			//rk - avoid casting to ilist. generics?
		    var results = AuditReader().CreateQuery()
		                .ForRevisionsOfEntity(typeof(StrTestEntity), false, true)
		                .Add(AuditEntity.Id().Eq(id))
		                .GetResultList();
			Assert.AreEqual(RevisionType.Added, ((IList)results[0])[2]);
			Assert.AreEqual(RevisionType.Modified, ((IList)results[1])[2]);
		}
	}
}