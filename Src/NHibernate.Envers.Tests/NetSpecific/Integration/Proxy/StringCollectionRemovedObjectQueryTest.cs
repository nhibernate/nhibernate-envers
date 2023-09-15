using System.Collections.Generic;
using System.Linq;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Entities.Collection;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Proxy
{
	//tests OneEntityQueryGenerator
	public partial class StringCollectionRemovedObjectQueryTest : TestBase
	{
		private int parentId;

		public StringCollectionRemovedObjectQueryTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var parent = new StringSetEntity();

			//Revision 1
			using (var tx = Session.BeginTransaction())
			{
				parentId = (int)Session.Save(parent);
				tx.Commit();
			}

			//Revision 2
			using (var tx = Session.BeginTransaction())
			{
				parent.Strings.Add("2");
				tx.Commit();
			}

			//Revision 3
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(parent);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyHistoryOfParent()
		{
			AuditReader().Find<StringSetEntity>(parentId, 1)
						 .Strings.Should().Be.Empty();
			AuditReader().Find<StringSetEntity>(parentId, 2)
						 .Strings.Single().Should().Be.EqualTo("2");
			var rev3 = AuditReader().CreateQuery().ForRevisionsOfEntity(typeof(StringSetEntity), true, true)
									 .Add(AuditEntity.Id().Eq(parentId))
									 .Add(AuditEntity.RevisionNumber().Eq(3))
									 .GetSingleResult<StringSetEntity>();
			rev3.Strings.Single().Should().Be.EqualTo("2");
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.StoreDataAtDelete, true);
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Entities.Collection.Mapping.hbm.xml" }; }
		}
	}
}