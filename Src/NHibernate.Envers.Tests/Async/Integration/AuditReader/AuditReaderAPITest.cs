﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NHibernate.Envers.Exceptions;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.AuditReader
{
	using System.Threading.Tasks;
	public partial class AuditReaderAPITest : TestBase
	{

		[Test]
		public async Task ShouldAuditAsync()
		{
			AuditReader().IsEntityClassAudited(typeof (AuditedTestEntity))
				.Should().Be.True();
			AuditReader().IsEntityNameAudited(typeof(AuditedTestEntity).FullName)
				.Should().Be.True();
			(await (AuditReader().GetRevisionsAsync(typeof(AuditedTestEntity),1)).ConfigureAwait(false))
				.Should().Have.SameSequenceAs(1, 2);
		}
	}
}