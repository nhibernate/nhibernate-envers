﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.DynamicComponent
{
	using System.Threading.Tasks;
	public partial class NonExistingKeyTest : TestBase
	{

		[Test]
		public async Task VerifyRevision1Async()
		{
			var rev = await (AuditReader().FindAsync<DynamicTestEntity>(id, 1)).ConfigureAwait(false);
			rev.Properties.Should().Be.Null();
		}

		[Test]
		public async Task VerifyRevision2Async()
		{
			var rev = await (AuditReader().FindAsync<DynamicTestEntity>(id, 2)).ConfigureAwait(false);
			rev.Properties["Prop1"].Should().Be.EqualTo(1);
			rev.Properties["Prop2"].Should().Be.EqualTo(2);
			rev.Properties["Prop3"].Should().Be.EqualTo(3);
		}

		[Test]
		public async Task VerifyRevision3Async()
		{
			var rev = await (AuditReader().FindAsync<DynamicTestEntity>(id, 3)).ConfigureAwait(false);
			rev.Properties.Contains("Prop1").Should().Be.False();
			rev.Properties["Prop2"].Should().Be.EqualTo(2);
			rev.Properties["Prop3"].Should().Be.EqualTo(3);
		}

		[Test]
		public async Task VerifyRevision4Async()
		{
			var rev = await (AuditReader().FindAsync<DynamicTestEntity>(id, 4)).ConfigureAwait(false);
			rev.Properties.Should().Be.Null();
		}
	}
}