using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Flush
{
	[TestFixture]
	public class DoubleFlushAddModTest :TestBase
	{
		private int id;

		public DoubleFlushAddModTest(string strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Mapping.hbm.xml" }; }
		}

		protected override FlushMode FlushMode
		{
			get { return FlushMode.Never; }
		}

		protected override void Initialize()
		{
			var fe = new StrTestEntity { Str = "x" };
			using (var tx = Session.BeginTransaction())
			{
				id = (int) Session.Save(fe);
				Session.Flush();
				fe.Str = "y";
				Session.Flush();
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
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
		public void VerifyHistoryOfId1()
		{
			var ver1 = new StrTestEntity { Id = id, Str = "y"};
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