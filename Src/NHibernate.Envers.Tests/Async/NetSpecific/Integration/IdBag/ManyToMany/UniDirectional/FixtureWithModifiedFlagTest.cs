﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using NHibernate.Envers.Configuration;
using NUnit.Framework;
using SharpTestsEx;
using NHibernate.Cfg;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.IdBag.ManyToMany.UniDirectional
{
	using System.Threading.Tasks;
	public partial class FixtureWithModifiedFlagTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionsCountsAsync()
		{
			(await (AuditReader().GetRevisionsAsync(typeof(UniOwning), owningId)).ConfigureAwait(false))
				.Should().Have.SameSequenceAs(1, 2);
			(await (AuditReader().GetRevisionsAsync(typeof(UniOwned), ownedId)).ConfigureAwait(false))
				.Should().Have.SameSequenceAs(2);
		}
	}
}