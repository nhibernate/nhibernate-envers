using System.Collections.Generic;
using NHibernate.Envers.Tests.Integration.NotUpdatable;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.NotUpdatable
{
	public partial class DetachedUpdateTest : TestBase
	{
		private long id;

		public DetachedUpdateTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
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
				id = (long)Session.Save(entity);
				tx.Commit();
			}
			ForceNewSession();
			//Revision 2
			using (var tx = Session.BeginTransaction())
			{
				entity.Data = "data2";
				entity.ConstantData1 = null;
				Session.Update(entity);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			AuditReader().GetRevisions(typeof(PropertyNotUpdatableEntity), id)
				.Should().Have.SameSequenceAs(1, 2);
		}

		[Test, Ignore("Currently prev state isn't available when update=false is used and user has changed entity state when detached.")]
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
				Data = "data2",
				ConstantData1 = "constant data 1",
				ConstantData2 = "constant data 2"
			};

			AuditReader().Find<PropertyNotUpdatableEntity>(id, 1)
				.Should().Be.EqualTo(ver1);
			AuditReader().Find<PropertyNotUpdatableEntity>(id, 2)
				.Should().Be.EqualTo(ver2);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Integration.NotUpdatable.Mapping.hbm.xml" };
			}
		}
	}
}