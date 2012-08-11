using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.Ids;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.Integration.Ids
{
	/// <summary>
	/// A test checking that when using Envers it is possible to have non-audited entities that use unsupported
	/// components in their ids, e.g. a many-to-one join to another entity.
	/// </summary>
	public class ManyToOneIdNotAuditedTest : TestBase
	{
		private ManyToOneNotAuditedEmbId id;

		public ManyToOneIdNotAuditedTest(string strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var uste = new UnversionedStrTestEntity {Str = "test1"};
			//rev 1
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(uste);
				tx.Commit();
			}
			id = new ManyToOneNotAuditedEmbId{Id = uste};
			//rev 2
			using (var tx = Session.BeginTransaction())
			{
				var mtoinate = new ManyToOneIdNotAuditedTestEntity {Data = "data1", Id = id};
				Session.Save(mtoinate);
				tx.Commit();
			}
		}

		[Test]
		public void DontCrash()
		{
			
		}

		protected override IEnumerable<string> Mappings
		{
			get { return new[] { "Integration.Ids.ManyToOneIdNotAuditedTest.hbm.xml", "Entities.Mapping.hbm.xml" }; }
		}
	}
}