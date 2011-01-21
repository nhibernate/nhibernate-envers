using System.Collections.Generic;
using System.Reflection;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Envers.Tests
{
	public abstract class TestBase
	{
		protected const string TestAssembly = "NHibernate.Envers.Tests";
		protected Cfg.Configuration Cfg { get; private set; }
		protected ISession Session { get; set; }
		protected IAuditReader AuditReader { get; set; }
		private ISessionFactory SessionFactory { get; set; }

		[SetUp]
		public void BaseSetup()
		{
			Cfg = new Cfg.Configuration();
			Cfg.Configure();
			addMappings();
			Cfg.IntegrateWithEnvers();
			AddToConfiguration(Cfg);
			SessionFactory = Cfg.BuildSessionFactory();
			using (Session = openSession(SessionFactory))
			{
				Initialize();				
			}
			Session = openSession(SessionFactory);
			AuditReader = AuditReaderFactory.Get(Session);
		}

		protected virtual void AddToConfiguration(Cfg.Configuration configuration){}

		private ISession openSession(ISessionFactory sessionFactory)
		{
			var session = sessionFactory.OpenSession();
			session.FlushMode = FlushMode;
			return session;
		}

		[TearDown]
		public void BaseTearDown()
		{
			if (Session != null)
			{
				Session.Close();
			}
			if (SessionFactory != null)
			{
				SessionFactory.Close();
			}
		}

		protected virtual FlushMode FlushMode
		{
			get { return FlushMode.Auto; }
		}

		protected abstract void Initialize();

		private string nameSpaceAssemblyExtracted()
		{
			var fullNamespace = GetType().Namespace;
			return fullNamespace.TrimStart(TestAssembly.ToCharArray());
		}

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

		private void addMappings()
		{
			var ass = Assembly.Load(TestAssembly);
			foreach (var mapping in Mappings)
			{
				Cfg.AddResource(TestAssembly + "." + mapping, ass);
			}
		}
	}
}