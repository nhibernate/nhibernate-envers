using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Bidirectional
{
	public class TwoRelationEndsSameColumnTest
	{
		[Test]
		public void ShouldIntegrateWithEnversNotThrowMappingException()
		{
			Assert.DoesNotThrow(() => getConfiguration());		
		}

		[Test]
		public void ShouldAuditModelProperly()
		{
			var cfg = getConfiguration();
			var session = getSession(cfg);

			const int modelId = 112;
			const int modelConfigurationId = 2987;

			var model = new Model { Id = modelId, ModelConfigurations = new HashSet<ModelConfigurationShared>() };
			var modelShared = new ModelShared { Id = modelId, ModelConfigurations = new HashSet<ModelConfigurationShared>() };
			var modelConfigurationShared = new ModelConfigurationShared { Id = modelConfigurationId };

			using (var tx = session.BeginTransaction())
			{
				session.Save(model);
				session.Save(modelShared);
				session.Save(modelConfigurationShared);

				model.ModelConfigurations.Add(modelConfigurationShared);
				modelShared.ModelConfigurations.Add(modelConfigurationShared);
				modelConfigurationShared.Model = modelShared;

				session.Save(model);
				session.Save(modelShared);
				session.Save(modelConfigurationShared);

				tx.Commit();
			}

			var modelFromAudit = session.Auditer().CreateQuery()
										.ForRevisionsOf<Model>().Single();

			Assert.AreEqual(model.Id, modelFromAudit.Id);
			Assert.AreEqual(modelConfigurationShared.Id, modelFromAudit.ModelConfigurations.Single().Id);
			Assert.AreEqual(modelShared.Id, modelFromAudit.ModelConfigurations.Single().Model.Id);
		}

		private Cfg.Configuration getConfiguration()
		{
			var cfg = new Cfg.Configuration();

			cfg.Configure();
			cfg.AddResource("NHibernate.Envers.Tests.NetSpecific.UnitTests.Bidirectional.Mapping.hbm.xml", GetType().Assembly);
			cfg.IntegrateWithEnvers();

			return cfg;
		}

		private static ISession getSession(Cfg.Configuration cfg)
		{
			var sessionFactory = cfg.BuildSessionFactory();
			var session = sessionFactory.OpenSession();
			session.FlushMode = FlushMode.Auto;
			return session;
		}
	}
}
