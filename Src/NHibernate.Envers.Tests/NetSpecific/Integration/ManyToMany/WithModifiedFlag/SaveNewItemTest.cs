using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tests.Integration.Inheritance.Entities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.ManyToMany.WithModifiedFlag
{
	public partial class SaveNewItemTest : TestBase
	{
		private long id;

		public SaveNewItemTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			using (var tx = Session.BeginTransaction())
			{
				var person = new Person { Name = "thename", Group = "something" };
				id = (long) Session.Save(person);
				tx.Commit();
			}
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.GlobalWithModifiedFlag, true);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Integration.Inheritance.Joined.Relation.Mapping.hbm.xml" };
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			AuditReader().GetRevisions(typeof(Person), id).Should().Have.SameSequenceAs(1);
		}
	}
}