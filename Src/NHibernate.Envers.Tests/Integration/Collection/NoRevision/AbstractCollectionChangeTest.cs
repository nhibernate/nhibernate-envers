using System.Collections.Generic;
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Collection.NoRevision
{
	public abstract partial class AbstractCollectionChangeTest : TestBase
	{
		protected AbstractCollectionChangeTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetEnversProperty(ConfigurationKey.RevisionOnCollectionChange, RevisionOnCollectionChange);
		}

		protected override void Initialize()
		{
			var n = new Name{TheName="name1"};
			var p = new Person();
			p.Names.Add(n);

			//Rev 1
			using (var tx = Session.BeginTransaction())
			{
				PersonId = (int) Session.Save(p);
				tx.Commit();
			}

			//Rev 2
			using (var tx = Session.BeginTransaction())
			{
				n.TheName = "Changed name";
				tx.Commit();
			}

			//Rev 3
			using (var tx = Session.BeginTransaction())
			{
				var n2 = new Name {TheName = "name2"};
				p.Names.Add(n2);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyPersonRevisionCount() 
		{
			AuditReader().GetRevisions(typeof(Person),PersonId)
				.Should().Have.SameSequenceAs(ExpectedPersonRevisions);
		}

		private int PersonId { get; set; }
		protected abstract bool RevisionOnCollectionChange { get; }
		protected abstract IEnumerable<long> ExpectedPersonRevisions { get; }
	}
}