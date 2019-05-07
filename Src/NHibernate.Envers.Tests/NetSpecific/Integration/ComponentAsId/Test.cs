using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Envers.Configuration;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.ComponentAsId
{
	public class ComponentAsId
	{
		private static ISessionFactory _sessionFactory;

		[Test]
		public void TestMethod()
		{
			CreateDatabase();
			InsertAndRemove();
		}

		static void CreateDatabase()
		{
			var configuration = new NHibernate.Cfg.Configuration();
			var _mapper = new CustomModelMapper();

			configuration.DataBaseIntegration((x) =>
			{
				x.ConnectionString = @"Data Source=localhost;Initial Catalog=envers;Integrated Security=SSPI";
				x.Dialect<MsSql2012Dialect>();
			});

			var types = typeof(Entity<>)
				.Assembly
				.GetTypes()
				.Where(t => !t.IsInterface && !t.IsGenericType && t.GetInterface("IComponentAsIdFlag", true) != null)
				;

			_mapper.AddMappings(types);

			configuration.AddMapping(_mapper.CompileMappingFor(types));

			configuration.SetEnversProperty(ConfigurationKey.AuditTableSuffix, "_A");
			configuration.SetEnversProperty(ConfigurationKey.StoreDataAtDelete, true);
			configuration.SetEnversProperty(ConfigurationKey.RevisionOnCollectionChange, false);

			var enversConf = new NHibernate.Envers.Configuration.Fluent.FluentConfiguration();
			enversConf.Audit(types);

			configuration.IntegrateWithEnvers(enversConf);


			var exporter = new SchemaExport(configuration);
			exporter.Execute(true, true, false);

			_sessionFactory = configuration.BuildSessionFactory();
		}

		static void InsertAndRemove()
		{
			void Save(object o)
			{
				using (ISession session = _sessionFactory.OpenSession())
				{
					using (var tran = session.BeginTransaction())
					{
						session.Save(o);
						session.Flush();
						tran.Commit();
					}
				}
			}

			void Del(object o)
			{
				using (ISession session = _sessionFactory.OpenSession())
				{
					using (var tran = session.BeginTransaction())
					{
						session.Delete(o);
						session.Flush();
						tran.Commit();
					}
				}
			}

			var udfDef = new UDFDef
			{
				Id = 1,
				SomeCol0 = "EEE"
			};

			Save(udfDef);

			var someEnt = new SomeEntity()
			{
				Id = 1,
				SomeCol2 = "RRR"
			};

			Save(someEnt);

			var udf = new SomeEntUDF
			{
				Id = new UdfId<UDFDef, SomeEntity>
				{
					UDFDef = udfDef,
					UDFOwnr = someEnt
				},
				SomeCol1 = "TTT"
			};

			Save(udf);

			Del(udf);
			Del(udfDef); // Error is here with Audit FK
		}
	}
}
