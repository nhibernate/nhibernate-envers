﻿using Iesi.Collections.Generic;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Join.NoAuditedProperties
{
	public class NoAuditedCollectionTest : TestBase
	{
		private const int id = 44;

		public NoAuditedCollectionTest(string strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var audited = new Audited
			              	{
			              		Id = id, 
									Number = 1, 
									XCollection = new HashedSet<NotAudited> {new NotAudited()}
			              	};
			//revision1
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(audited);
				tx.Commit();
			}
			//revision 2
			using (var tx = Session.BeginTransaction())
			{
				audited.Number = 2;
				audited.XCollection.Add(new NotAudited());
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevision1()
		{
			var rev1 = AuditReader().Find<Audited>(id, 1);
			rev1.Number.Should().Be.EqualTo(0);
			rev1.XCollection.Should().Be.Null();
		}

		[Test]
		public void VerifyRevision2()
		{
			var rev1 = AuditReader().Find<Audited>(id, 2);
			rev1.Number.Should().Be.EqualTo(0);
			rev1.XCollection.Should().Be.Null();
		}

		[Test]
		public void ShouldHaveNotCreatedAuditedJoinTable()
		{
			Cfg.GetClassMapping("NHibernate.Envers.Tests.NetSpecific.Integration.Join.NoAuditedProperties.Audited_AUD")
				.JoinIterator
				.Should().Be.Empty();
		}
	}
}