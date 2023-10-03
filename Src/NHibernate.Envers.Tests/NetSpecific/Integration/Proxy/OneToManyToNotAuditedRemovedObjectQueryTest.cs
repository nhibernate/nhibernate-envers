using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Integration.EntityNames.OneToManyNotAudited;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Proxy
{
	//TwoEntityOneAuditedQueryGenerator
	public partial class OneToManyToNotAuditedRemovedObjectQueryTest : TestBase
	{
		private long parentId;

		public OneToManyToNotAuditedRemovedObjectQueryTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var parent = new Car {Owners = new List<Person> {new Person {Name = "Roger1"}}};

			//Revision 1
			using (var tx = Session.BeginTransaction())
			{
				parentId = (long)Session.Save(parent);
				tx.Commit();
			}

			//Revision 2 - no revision
			using (var tx = Session.BeginTransaction())
			{
				parent.Owners.Single().Name = "Roger2";
				tx.Commit();
			}

			//Revision 2
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(parent.Owners.Single());
				Session.Delete(parent);
				tx.Commit();
			}
		}


		[Test]
		public void VerifyHistoryOfParent()
		{
			AuditReader().Find<Car>(parentId, 1).Owners.Should().Be.Empty();
			AuditReader().CreateQuery().ForRevisionsOfEntity(typeof (Car), true, true)
			             .Add(AuditEntity.Id().Eq(parentId))
			             .Add(AuditEntity.RevisionNumber().Eq(2))
			             .GetSingleResult<Car>()
			             .Owners.Should().Be.Empty();
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.StoreDataAtDelete, true);
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Integration.EntityNames.OneToManyNotAudited.Mapping.hbm.xml" }; }
		}
	}
}