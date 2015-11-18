
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using NHibernate.Cfg;
using NHibernate.Dialect;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Event;
using NHibernate.Envers.Query;
using NHibernate.Envers.Query.Criteria;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Set
{
    public class MergeColModTest
    {
        public MergeColModTest()
        {
            StrategyType = System.Type.GetType("NHibernate.Envers.Strategy.ValidityAuditStrategy, NHibernate.Envers");
        }

        int casId1;

        int casId2;

        protected void Initialize()
        {
            CreateCase1();
            CreateCase2();

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
                    Assert.IsTrue(is_mod,"mod flag was false");
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

        protected string TestAssembly { get; private set; }
        protected Cfg.Configuration Cfg { get; private set; }
        protected ISession Session { get; private set; }
        protected System.Type StrategyType { get; private set; }
        private ISessionFactory SessionFactory { get; set; }
        private IAuditReader _auditReader;

        

        [SetUp]
        public void BaseSetup()
        {
            TestAssembly = GetType().Assembly.GetName().Name;
            Cfg = new Cfg.Configuration();
            Cfg.SetEnversProperty(ConfigurationKey.AuditStrategy, StrategyType);
            Cfg.SetEnversProperty(ConfigurationKey.GlobalWithModifiedFlag, true);
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

        protected virtual void AddToConfiguration(Cfg.Configuration configuration) { }

        protected int MillisecondPrecision
        {
            get { return Dialect is MySQLDialect ? 1000 : 100; }
        }

        protected virtual FlushMode FlushMode
        {
            get { return FlushMode.Auto; }
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
