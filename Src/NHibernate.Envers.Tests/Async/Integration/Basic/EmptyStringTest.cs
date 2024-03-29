﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using NHibernate.Dialect;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Basic
{
	using System.Threading.Tasks;
	public partial class EmptyStringTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountsAsync()
		{
			(await (AuditReader().GetRevisionsAsync(typeof (StrTestEntity), emptyId)).ConfigureAwait(false)).Should().Have.SameSequenceAs(1);
			(await (AuditReader().GetRevisionsAsync(typeof (StrTestEntity), nullId)).ConfigureAwait(false)).Should().Have.SameSequenceAs(1);
		}
	}
}