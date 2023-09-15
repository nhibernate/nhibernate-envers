using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.BidirectionalSameColumn
{
	public partial class TwoRelationEndsSameColumnTest : TestBase
	{
		private const int modelId = 112;
		private const int modelConfigurationId = 2987;

		public TwoRelationEndsSameColumnTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var model = new Model { Id = modelId, ModelConfigurations = new HashSet<ModelConfigurationShared>() };
			var modelShared = new ModelShared { Id = modelId, ModelConfigurations = new HashSet<ModelConfigurationShared>() };
			var modelConfigurationShared = new ModelConfigurationShared { Id = modelConfigurationId };

			using (var tx = Session.BeginTransaction())
			{
				Session.Save(model);
				Session.Save(modelShared);
				Session.Save(modelConfigurationShared);

				model.ModelConfigurations.Add(modelConfigurationShared);
				modelShared.ModelConfigurations.Add(modelConfigurationShared);
				modelConfigurationShared.Model = modelShared;

				Session.Save(model);
				Session.Save(modelShared);
				Session.Save(modelConfigurationShared);

				tx.Commit();
			}
		}


		[Test]
		public void ShouldAuditModelProperly()
		{
			var modelFromAudit = Session.Auditer().CreateQuery()
										.ForRevisionsOf<Model>().Single();

			Assert.AreEqual(modelId, modelFromAudit.Id);
			Assert.AreEqual(modelConfigurationId, modelFromAudit.ModelConfigurations.Single().Id);
			Assert.AreEqual(modelId, modelFromAudit.ModelConfigurations.Single().Model.Id);
		}
	}
}
