using System.Collections.Generic;
using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Event;
using NUnit.Framework;

namespace NHibernate.Envers.Tests
{
	public abstract class OneStrategyTestBase
	{
		protected string TestAssembly { get; private set; }
		protected Cfg.Configuration Cfg { get; private set; }
		protected ISession Session { get; private set; }
		protected System.Type StrategyType { get; private set; }
		private ISessionFactory SessionFactory { get; set; }
		private IAuditReader _auditReader;

		protected OneStrategyTestBase(string strategyType)
		{
			StrategyType = System.Type.GetType(strategyType);
		}

		[SetUp]
		public void BaseSetup()
		{
			TestAssembly = GetType().Assembly.GetName().Name;
			Cfg = new Cfg.Configuration();
			Cfg.SetEnversProperty(ConfigurationKey.AuditStrategy, StrategyType);
			AddToConfiguration(Cfg);
			Cfg.Configure();
			addMappings();
			Cfg.IntegrateWithEnvers(new AuditEventListener(), EnversConfiguration());
			SessionFactory = Cfg.BuildSessionFactory();
			var notRun = TestShouldNotRunMessage();
			if (!string.IsNullOrEmpty(notRun))
				Assert.Ignore(notRun);
			Session = openSession(SessionFactory);
			Initialize();
			closeSessionAndAuditReader();
			Session = openSession(SessionFactory);
		}

		[TearDown]
		public void BaseTearDown()
		{
			closeSessionAndAuditReader();
			AuditConfiguration.Remove(Cfg);
			if (SessionFactory != null)
			{
				SessionFactory.Close();
			}
		}

		protected virtual string TestShouldNotRunMessage()
		{
			return null;
		}

		protected Dialect.Dialect Dialect
		{
			get { return ((ISessionFactoryImplementor)SessionFactory).Dialect; }
		}

		protected virtual IMetaDataProvider EnversConfiguration()
		{
			return new AttributeConfiguration();
		}

		protected IAuditReader AuditReader()
		{
			return _auditReader ?? (_auditReader = Session.Auditer());
		}

		protected virtual void AddToConfiguration(Cfg.Configuration configuration){}

		protected int MillisecondPrecision
		{
			get { return Dialect is MySQLDialect ? 1000 : 100; }
		}

		protected virtual FlushMode FlushMode
		{
			get { return FlushMode.Auto; }
		}

		protected abstract void Initialize();


		protected virtual IEnumerable<string> Mappings
		{
			get
			{
				return new[]
				       	{
				       		nameSpaceAssemblyExtracted() + ".Mapping.hbm.xml"
				       	};
			}
		}

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
			Session = SessionFactory.OpenSession();
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
			if (Session != null)
				Session.Dispose();
			_auditReader = null;
		}
	}
}