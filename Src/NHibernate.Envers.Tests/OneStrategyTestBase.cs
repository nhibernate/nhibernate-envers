using System;
using System.Collections.Generic;
using System.Reflection;
using FirebirdSql.Data.FirebirdClient;
using NHibernate.Cfg;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Event;
using NHibernate.Envers.Strategy;
using NHibernate.Envers.Tests.Tools;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace NHibernate.Envers.Tests
{
	public abstract class OneStrategyTestBase
	{
		private ISessionFactoryImplementor _sessionFactory;
		private IAuditReader _auditReader;

		protected string TestAssembly { get; private set; }
		protected Cfg.Configuration Cfg { get; private set; }
		protected ISession Session { get; private set; }
		protected System.Type StrategyType { get; private set; }

		protected OneStrategyTestBase(AuditStrategyForTest strategyType)
		{
			setStrategyType(strategyType);
		}

		private void setStrategyType(AuditStrategyForTest strategyType)
		{
			switch (strategyType)
			{
				case AuditStrategyForTest.DefaultAuditStrategy:
					StrategyType = typeof(DefaultAuditStrategy);
					break;
				case AuditStrategyForTest.ValidityAuditStrategy:
					StrategyType = typeof(ValidityAuditStrategy);
					break;
				default:
					throw new ArgumentException();
			}
		}

		[SetUp]
		public void BaseSetup()
		{
			TestAssembly = GetType().Assembly.GetName().Name;
			Cfg = new Cfg.Configuration().SetEnversProperty(ConfigurationKey.AuditStrategy, StrategyType);
			AddToConfiguration(Cfg);
			Cfg.Configure().OverrideSettingsFromEnvironmentVariables();
			addMappings();
			Cfg.IntegrateWithEnvers(new AuditEventListener(), EnversConfiguration());
			createDb();
			_sessionFactory = (ISessionFactoryImplementor)Cfg.BuildSessionFactory();
			if (!testShouldRun())
				Assert.Ignore(TestShouldNotRunMessage());
			createDbSchema();
			Session = openSession(_sessionFactory);
			Initialize();
			closeSessionAndAuditReader();
			Session = openSession(_sessionFactory);
		}

		private void createDbSchema()
		{
			new SchemaExport(Cfg).Create(false, true);
		}
		
		
		private void dropDbSchema()
		{
			if (_sessionFactory.ConnectionProvider.Driver is FirebirdClientDriver)
			{
				// necessary firebird hack to be able to drop used tables
				FbConnection.ClearAllPools();
			}
			new SchemaExport(Cfg).Drop(false, true);
		}

		private static bool hasCreatedDatabase;
		private void createDb()
		{
			if (hasCreatedDatabase)
				return;
			hasCreatedDatabase = true;
			DatabaseSetup.CreateEmptyDatabase(Cfg);
		}

		[TearDown]
		public void BaseTearDown()
		{
			closeSessionAndAuditReader();
			AuditConfiguration.Remove(Cfg);
			if(testShouldRun())
				dropDbSchema();
			_sessionFactory?.Close();
		}

		private bool testShouldRun()
		{
			return TestShouldNotRunMessage() == null;
		}
		
		protected virtual string TestShouldNotRunMessage()
		{
			return null;
		}

		protected Dialect.Dialect Dialect => _sessionFactory.Dialect;

		protected virtual IMetaDataProvider EnversConfiguration()
		{
			return new AttributeConfiguration();
		}

		protected IAuditReader AuditReader()
		{
			return _auditReader ?? (_auditReader = Session.Auditer());
		}

		protected virtual void AddToConfiguration(Cfg.Configuration configuration){}

		protected virtual FlushMode FlushMode => FlushMode.Auto;

		protected abstract void Initialize();


		protected virtual IEnumerable<string> Mappings => new[]
		{
			nameSpaceAssemblyExtracted() + ".Mapping.hbm.xml"
		};

		private ISession openSession(ISessionFactory sessionFactory)
		{
			var session = sessionFactory.OpenSession();
			session.FlushMode = FlushMode;
			return session;
		}

		private string nameSpaceAssemblyExtracted()
		{
			var fullNamespace = GetType().Namespace;
			return fullNamespace.Remove(0, TestAssembly.Length + 1);
		}

		protected void ForceNewSession()
		{
			Session.Close();
			Session = _sessionFactory.OpenSession();
			_auditReader = null;
		}

		private void addMappings()
		{
			var ass = Assembly.Load(TestAssembly);
			foreach (var mapping in Mappings)
			{
				Cfg.AddResource(TestAssembly + "." + mapping, ass);
			}
		}

		private void closeSessionAndAuditReader()
		{
			Session?.Dispose();
			_auditReader = null;
		}
	}
}