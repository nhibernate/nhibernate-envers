using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Event;
using NHibernate.Envers.Strategy;
using NUnit.Framework;

namespace NHibernate.Envers.Tests
{
	public abstract class OneStrategyTestBase
	{
		private ISessionFactory _sessionFactory;
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
			Cfg = new Cfg.Configuration();
			Cfg.SetEnversProperty(ConfigurationKey.AuditStrategy, StrategyType);
			AddToConfiguration(Cfg);
			Cfg.Configure(configurationFile());
			addMappings();
			Cfg.IntegrateWithEnvers(new AuditEventListener(), EnversConfiguration());
			_sessionFactory = Cfg.BuildSessionFactory();
			var notRun = TestShouldNotRunMessage();
			if (!string.IsNullOrEmpty(notRun))
				Assert.Ignore(notRun);
			Session = openSession(_sessionFactory);
			Initialize();
			closeSessionAndAuditReader();
			Session = openSession(_sessionFactory);
		}

		[TearDown]
		public void BaseTearDown()
		{
			closeSessionAndAuditReader();
			AuditConfiguration.Remove(Cfg);
			_sessionFactory?.Close();
		}

		protected virtual string TestShouldNotRunMessage()
		{
			return null;
		}

		protected Dialect.Dialect Dialect => ((ISessionFactoryImplementor)_sessionFactory).Dialect;

		protected virtual IMetaDataProvider EnversConfiguration()
		{
			return new AttributeConfiguration();
		}

		protected IAuditReader AuditReader()
		{
			return _auditReader ?? (_auditReader = Session.Auditer());
		}

		protected virtual void AddToConfiguration(Cfg.Configuration configuration){}

		protected int MillisecondPrecision => Dialect is MySQLDialect ? 1000 : 100;

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
		
		private static string configurationFile()
		{
			var baseDir = AppDomain.CurrentDomain.BaseDirectory;
			var relativeSearchPath = AppDomain.CurrentDomain.RelativeSearchPath;
			var folder = relativeSearchPath == null ? baseDir : Path.Combine(baseDir, relativeSearchPath);

			return Path.Combine(folder, "hibernate.cfg.xml");
		}
	}
}