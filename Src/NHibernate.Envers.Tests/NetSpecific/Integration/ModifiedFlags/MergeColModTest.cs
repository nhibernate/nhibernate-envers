using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.ModifiedFlags
{
	public partial class MergeColModTest : TestBase
	{
		public MergeColModTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			CreateCase1();
			CreateCase2();
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.GlobalWithModifiedFlag, true);
		}

		private void CreateCase1()
		{
			Casee cas = new Casee();
			CaseToCaseTag ctc = new CaseToCaseTag();

			using (var tx = Session.BeginTransaction())
			{
				ctc.Right = cas;
				cas.CaseTags = new HashSet<CaseToCaseTag>();
				cas.CaseTags.Add(ctc);

				Session.Save(cas);
				tx.Commit();
			}

			cas.CaseTags.Remove(ctc);
			cas.LastModifyDate = DateTime.UtcNow.AddHours(-5);
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(cas);
				tx.Commit();
			}
		}

		private void CreateCase2()
		{
			Casee cas = new Casee();
			CaseToCaseTag ctc = new CaseToCaseTag();

			using (var tx = Session.BeginTransaction())
			{

				ctc.Right = cas;
				cas.CaseTags = new HashSet<CaseToCaseTag>();
				cas.CaseTags.Add(ctc);

				Session.Save(cas);
				tx.Commit();
			}

			cas.CaseTags.Remove(ctc);
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(cas);
				tx.Commit();
			}
		}

		[Test]
		public void CheckHistory()
		{
			var changedRevisions = AuditReader().CreateQuery()
				.ForHistoryOf<Casee, DefaultRevisionEntity>()
				.Add(AuditEntity.Property("CaseTags").HasChanged())
				.Results();
			//rev 2 and rev 4 are the revisions where we remove the tag from the case, thus both revisions should have the casetag property marked as being changed
			Assert.IsTrue(changedRevisions.Count(x => x.RevisionEntity.Id == 2 || x.RevisionEntity.Id == 4) == 2);
		}

	}
}
