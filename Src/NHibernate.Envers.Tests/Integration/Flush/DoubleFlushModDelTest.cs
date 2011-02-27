using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Flush
{
	[TestFixture]
	public class DoubleFlushModDelTest :TestBase
	{
		private int id;

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
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				fe.Str = "y";
				Session.Flush();
				Session.Delete(fe);
				Session.Flush();
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions<StrTestEntity>( id));
		}

		[Test]
		public void VerifyHistoryOfId()
		{
			var ver1 = new StrTestEntity { Id = id, Str = "x"};

			Assert.AreEqual(ver1, AuditReader().Find<StrTestEntity>(id, 1));
			Assert.IsNull(AuditReader().Find<StrTestEntity>(id, 2));
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
			Assert.AreEqual(RevisionType.Deleted, ((IList)results[1])[2]);
		}
	}
}