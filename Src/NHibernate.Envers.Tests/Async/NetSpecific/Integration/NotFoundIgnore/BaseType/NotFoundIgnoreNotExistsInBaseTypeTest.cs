﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.NotFoundIgnore.BaseType
{
	using System.Threading.Tasks;
	public partial class NotFoundIgnoreNotExistsInBaseTypeTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountAsync()
		{
			(await (AuditReader().GetRevisionsAsync(typeof(Parent), parentId)).ConfigureAwait(false))
				.Should().Have.SameSequenceAs(1);
		}

		[Test]
		public async Task VerifyHistoryAsync()
		{
			(await (AuditReader().FindAsync<Parent>(parentId, 1)).ConfigureAwait(false)).Children
				.Single().Name.Should().Be.Null();
		}
	}
}