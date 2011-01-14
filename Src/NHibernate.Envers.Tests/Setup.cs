using System;
using System.Data.SqlClient;
using System.IO;
using System.Text.RegularExpressions;
using log4net;
using NUnit.Framework;

namespace NHibernate.Envers.Tests
{
	[SetUpFixture]
	public class Setup
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(Setup));

		[SetUp]
		public void RunOnce()
		{
			var log4netConf = new FileInfo(Environment.CurrentDirectory + @"\log4net.xml");
			log4net.Config.XmlConfigurator.Configure(log4netConf);

			SetupDatabase();
		}

		private static void SetupDatabase()
		{
			var connStr = new Cfg.Configuration().Configure().Properties[Cfg.Environment.ConnectionString];

			using (var conn = new SqlConnection(connStr))
			{
				conn.Open();

				using (var cmd = new System.Data.SqlClient.SqlCommand("use master", conn))
				{
					cmd.ExecuteNonQuery();
					var databaseName = GetDatabaseNameFromConnectionString(connStr);
					cmd.CommandText = string.Format("drop database {0}", databaseName);

					try
					{
						cmd.ExecuteNonQuery();
					}
					catch (Exception e)
					{
						log.Warn("Couldn't drop database. " + e.Message);
					}

					cmd.CommandText = string.Format("create database {0}", databaseName);
					cmd.ExecuteNonQuery();
				}
			}
		}

		private static string GetDatabaseNameFromConnectionString(string cs)
		{
			var match = Regex.Match(cs, @"(Initial Catalog=|Database=)([^;]*);").Groups;
			return match[2].Value;
		}
	}
}