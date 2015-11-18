
using System;
using System.Collections.Generic;
using System.Data;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Set
{
	public class MergeColModTest : TestBase
	{
		public MergeColModTest(string strategyType) : base(strategyType)
		{
		}
		int casId1;

		int casId2;

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

				casId1 = (int)Session.Save(cas);
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

				casId1 = (int)Session.Save(cas);
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
		public void CheckHistoryWithMerge()
		{
			var cmd = Session.Connection.CreateCommand();
			cmd.CommandText = "select CaseTags_MOD from casee_aud where rev = 2";

			using (IDataReader dr = cmd.ExecuteReader())
			{
				while (dr.Read())
				{
					var is_mod = dr.GetBoolean(0);
					Assert.IsTrue(is_mod, "mod flag was false");
				}
			}
		}

		[Test]
		public void CheckHistoryWithoutMerge()
		{
			var cmd = Session.Connection.CreateCommand();
			cmd.CommandText = "select CaseTags_MOD from casee_aud where rev = 4";

			using (IDataReader dr = cmd.ExecuteReader())
			{
				while (dr.Read())
				{
					var is_mod = dr.GetBoolean(0);
					Assert.IsTrue(is_mod, "mod flag was false");
				}
			}
		}
	}
}
