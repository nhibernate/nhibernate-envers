using System.Linq;
using NHibernate.Envers.Configuration;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.NotUpdatable
{
	public partial class PropertyNotUpdatableTest : TestBase
	{
		private long id;

		public PropertyNotUpdatableTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			ConfigurationKey.StoreDataAtDelete.SetUserValue(configuration, true);
		}

		protected override void Initialize()
		{
			var entity = new PropertyNotUpdatableEntity
			             	{
			             		Data = "data", 
									ConstantData1 = "constant data 1", 
									ConstantData2 = "constant data 2"
			             	};
			//Revision 1
			using (var tx = Session.BeginTransaction())
			{
				id = (long) Session.Save(entity);
				tx.Commit();
			}
			//Revision 2
			using (var tx = Session.BeginTransaction())
			{
				entity.Data = "modified data";
				entity.ConstantData1 = null;
				tx.Commit();
			}
			//Revision 3
			ForceNewSession();
			using (var tx = Session.BeginTransaction())
			{
				entity.Data = "another modified data";
				entity.ConstantData2 = "invalid data";
				Session.Merge(entity);
				tx.Commit();
			}
			//Revision 4
			ForceNewSession();
			using (var tx = Session.BeginTransaction())
			{
				Session.Refresh(entity);
				Session.Delete(entity);
				tx.Commit();
			}
		}
		
		[Test]
		public void VerifyRevisionCount()
		{
			AuditReader().GetRevisions(typeof (PropertyNotUpdatableEntity), id)
				.Should().Have.SameSequenceAs(1, 2, 3, 4);
		}

		[Test]
		public void VerifyHistoryOfId()
		{
			var ver1 = new PropertyNotUpdatableEntity
			           	{
			           		Id = id,
			           		Data = "data",
			           		ConstantData1 = "constant data 1",
			           		ConstantData2 = "constant data 2"
			           	};
			var ver2 = new PropertyNotUpdatableEntity
			           	{
			           		Id = id,
								Data = "modified data",
			           		ConstantData1 = "constant data 1",
			           		ConstantData2 = "constant data 2"
			           	};
			var ver3 = new PropertyNotUpdatableEntity
			           	{
			           		Id = id,
								Data = "another modified data",
			           		ConstantData1 = "constant data 1",
			           		ConstantData2 = "constant data 2"
			           	};

			AuditReader().Find<PropertyNotUpdatableEntity>(id, 1)
				.Should().Be.EqualTo(ver1);
			AuditReader().Find<PropertyNotUpdatableEntity>(id, 2)
				.Should().Be.EqualTo(ver2);
			AuditReader().Find<PropertyNotUpdatableEntity>(id, 3)
				.Should().Be.EqualTo(ver3);
		}

		[Test]
		public void VerifyDeleteState()
		{
			var delete = new PropertyNotUpdatableEntity
			             	{
			             		Id = id,
			             		Data = "another modified data",
			             		ConstantData1 = "constant data 1",
			             		ConstantData2 = "constant data 2"
			             	};
			AuditReader().CreateQuery().ForRevisionsOf<PropertyNotUpdatableEntity>(true).Results().Last()
				.Should().Be.EqualTo(delete);
		}
	}
}