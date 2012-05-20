using System.Collections.Generic;
using NHibernate.Envers.Exceptions;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Query
{
	public class QueryForNonAuditEntityTest : TestBase
	{
		public QueryForNonAuditEntityTest(string strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new string[0];
			}
		}

		protected override void Initialize()
		{
		}

		[Test]
		public void ShouldThrowWhenFind()
		{
			Assert.Throws<NotAuditedException>(() =>
			            AuditReader().Find<QueryForNonAuditEntityTest>(1, 1)
				);
		}

		[Test]
		public void ShouldThrowWhenQueryIsCreated()
		{
			Assert.Throws<NotAuditedException>(() =>
			            AuditReader().CreateQuery().ForEntitiesAtRevision(typeof (QueryForNonAuditEntityTest), 1)
				);
		}

		[Test]
		public void ShouldThrowWhenGenericQueryIsCreated()
		{
			Assert.Throws<NotAuditedException>(() =>
							AuditReader().CreateQuery().ForEntitiesAtRevision<QueryForNonAuditEntityTest>(1)
				);
		}

	}
}