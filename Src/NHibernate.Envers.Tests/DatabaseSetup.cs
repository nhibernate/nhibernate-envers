using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using FirebirdSql.Data.FirebirdClient;
using NHibernate.Envers.Tests.Tools;
using Npgsql;

namespace NHibernate.Envers.Tests
{
	public class DatabaseSetup
	{
		private static readonly IDictionary<string, Action<Cfg.Configuration>> setupMethods = new Dictionary<string, Action<Cfg.Configuration>>
			{
				{"NHibernate.Driver.SqlClientDriver", setupSqlServer},
				{"NHibernate.Driver.MySqlDataDriver", setupMySql},
				{"NHibernate.Driver.FirebirdClientDriver", setupFirebird},
				{"NHibernate.Driver.NpgsqlDriver", setupNpgsql},
				{"NHibernate.Driver.OracleManagedDataClientDriver", setupOracle}
			};

		public static void CreateEmptyDatabase(Cfg.Configuration cfg)
		{
			var driver = cfg.Properties[Cfg.Environment.ConnectionDriver];

			setupMethods[driver](cfg);
		}
		
		private static void setupFirebird(Cfg.Configuration cfg)
		{
			var connStr = cfg.Properties[Cfg.Environment.ConnectionString];
			try
			{
				FbConnection.DropDatabase(connStr);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}
			// With UTF8 charset, string takes up to four times as many space, causing the
			// default page-size of 4096 to no more be enough for index key sizes. (Index key
			// size is limited to a quarter of the page size.)
			FbConnection.CreateDatabase(connStr, pageSize:16384, forcedWrites:false);
		}
		
		private static void setupNpgsql(Cfg.Configuration cfg)
		{
			var connStr = cfg.Properties[Cfg.Environment.ConnectionString].ReplaceCaseInsensitive("Database=envers", "Database=postgres");

			using (var conn = new NpgsqlConnection(connStr))
			{
				conn.Open();

				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "drop database envers";

					try
					{
						cmd.ExecuteNonQuery();
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
					}

					cmd.CommandText = "create database envers";
					cmd.ExecuteNonQuery();
				}
			}
		}

		private static void setupSqlServer(Cfg.Configuration cfg)
		{
			var connStr = cfg.Properties[Cfg.Environment.ConnectionString].ReplaceCaseInsensitive("initial catalog=envers", "initial catalog=master");

			using (var conn = new SqlConnection(connStr))
			{
				conn.Open();

				using (var cmd = conn.CreateCommand())
				{
					cmd.CommandText = "drop database envers";

					try
					{
						cmd.ExecuteNonQuery();
					}
					catch(Exception e)
					{
						Console.WriteLine(e);
					}

					cmd.CommandText = "create database envers";
					cmd.ExecuteNonQuery();
				}
			}
		}
		
		private static void setupMySql(Cfg.Configuration obj)
		{
			//do nothing
		}
		
		private static void setupOracle(Cfg.Configuration obj)
		{
			//do nothing
		}
	}
}