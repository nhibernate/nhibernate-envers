﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.OneToOne.BiDirectional
{
	using System.Threading.Tasks;
	public partial class BidirectionalSaveTest : TestBase
	{

		
		[Test]
		public async Task VerifyRevisionCountAsync()
		{
			(await (AuditReader().GetRevisionsAsync(typeof(BiRefEdEntity), id)).ConfigureAwait(false))
				.Should().Have.SameSequenceAs(1);
		}
		
		[Test]
		public async Task VerifyHistoryAsync()
		{
			(await (AuditReader().FindAsync<BiRefEdEntity>(id, 1)).ConfigureAwait(false)).Data.Should().Be.EqualTo("2");
		}
	}
}