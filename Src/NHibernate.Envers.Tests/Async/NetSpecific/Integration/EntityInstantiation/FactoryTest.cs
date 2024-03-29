﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NUnit.Framework;
using NHibernate.Envers.Configuration;
using NHibernate.Cfg;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.EntityInstantiation
{
	using System.Threading.Tasks;
	public partial class FactoryTest : TestBase
	{

		[Test]
		public async Task VerifyCreatedByFactoryAsync()
		{
			var ent = await (AuditReader().FindAsync<FactoryCreatedTestEntity>(id1, 1)).ConfigureAwait(false);
			Assert.IsTrue(ent.CreatedByFactory);
		}

		[Test]
		public async Task VerifyNotCreatedByFactoryAsync()
		{
			var ent = await (AuditReader().FindAsync<TestEntityWithContext>(id2, 1)).ConfigureAwait(false);
			Assert.IsFalse(ent.CreatedByFactory);
		}

		[Test]
		public async Task VerifyContextAssignedByListenerAsync()
		{
			var ent1 = await (AuditReader().FindAsync<FactoryCreatedTestEntity>(id1, 1)).ConfigureAwait(false);
			var ent2 = await (AuditReader().FindAsync<TestEntityWithContext>(id2, 1)).ConfigureAwait(false);

			Assert.IsNotNull(ent1.Context);
			Assert.IsNotNull(ent2.Context);
		}
	}
}
