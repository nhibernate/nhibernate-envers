using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Integration.ManyToMany.Ternary;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Proxy
{
	//tests ThreeEntityQueryGenerator
	public partial class MapRemovedObjectQueryTest : TestBase
	{
		private int id;

		public MapRemovedObjectQueryTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var entity = new TernaryMapEntity();
			var index = new IntTestPrivSeqEntity {Number = 1};
			var element = new StrTestPrivSeqEntity {Str = "1"};
			entity.Map[index] = element;

			//Revision 1
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(index);
				Session.Save(element);
				id = (int)Session.Save(entity);
				tx.Commit();
			}

			//Revision 2
			using (var tx = Session.BeginTransaction())
			{
				index.Number = 2;
				element.Str = "2";
				tx.Commit();
			}

			//Revision 3
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(entity);
				Session.Delete(index);
				Session.Delete(element);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyHistoryOfParent()
		{
			var rev3 = AuditReader().CreateQuery().ForRevisionsOfEntity(typeof(TernaryMapEntity), true, true)
									 .Add(AuditEntity.Id().Eq(id))
									 .Add(AuditEntity.RevisionNumber().Eq(3))
									 .GetSingleResult<TernaryMapEntity>();
			rev3.Map.Keys.Single().Number.Should().Be.EqualTo(2);
			rev3.Map.Values.Single().Str.Should().Be.EqualTo("2");
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.StoreDataAtDelete, true);
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Integration.ManyToMany.Ternary.Mapping.hbm.xml" }; }
		}
	}
}