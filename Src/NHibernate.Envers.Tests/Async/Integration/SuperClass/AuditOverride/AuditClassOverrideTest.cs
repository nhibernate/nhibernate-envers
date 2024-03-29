﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NHibernate.Mapping;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.SuperClass.AuditOverride
{
	using System.Threading.Tasks;
	public partial class AuditClassOverrideTest : TestBase
	{

		[Test]
		public async Task VerifyHistoryOfClassOverrideAuditedEntityAsync()
		{
			var ver1 = new ClassOverrideAuditedEntity {Id = classAuditedEntityId, Number1 = 1, Str1 = "data 1", Str2 = "data 2"};
			(await (AuditReader().FindAsync<ClassOverrideAuditedEntity>(classAuditedEntityId, 1)).ConfigureAwait(false))
				.Should().Be.EqualTo(ver1);
		}

		[Test]
		public async Task VerifyHistoryOfClassOverrideNotAuditedEntityAsync()
		{
			var ver1 = new ClassOverrideNotAuditedEntity { Id = classNotAuditedEntityId, Number1 = 0, Str1 = null, Str2 = "data 2" };
			(await (AuditReader().FindAsync<ClassOverrideNotAuditedEntity>(classNotAuditedEntityId, 2)).ConfigureAwait(false))
				.Should().Be.EqualTo(ver1);
		}
	}
}