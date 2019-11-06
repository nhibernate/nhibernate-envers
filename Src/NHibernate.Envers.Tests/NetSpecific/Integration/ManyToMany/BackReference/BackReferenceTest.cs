using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.ManyToMany.BackReference
{
	public class BackReferenceTest : TestBase
	{
		public BackReferenceTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings => new[] {"NetSpecific.Integration.ManyToMany.BackReference.Mapping.hbm.xml"};

		protected override void Initialize()
		{
		}

		[Test]
		public void VerifySave()
		{
			using (var tx = Session.BeginTransaction())
			{
				var item = new BackReferenceEntity();
				
				Session.Save(item);

				var entity = new OwningEntity
				{
					Items = { item }
				};
				
				Session.Save(entity);
				
				tx.Commit();
			}
		}
	}
}