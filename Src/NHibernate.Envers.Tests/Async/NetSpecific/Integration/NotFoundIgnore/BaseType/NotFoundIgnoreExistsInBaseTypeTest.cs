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
	public partial class NotFoundIgnoreExistsInBaseTypeTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountAsync()
		{
			(await (AuditReader().GetRevisionsAsync(typeof(Parent), parentId)).ConfigureAwait(false))
				.Should().Have.SameSequenceAs(1, 2);
		}

		[Test]
		public async Task VerifyHistoryAsync()
		{
			var ver1children = (await (AuditReader().FindAsync<Parent>(parentId, 1)).ConfigureAwait(false)).Children;
			var ver2children = (await (AuditReader().FindAsync<Parent>(parentId, 2)).ConfigureAwait(false)).Children;

			ver1children.Single().Sex.Should().Be.EqualTo("Boy");
			ver2children[0].Sex.Should().Be.EqualTo("Boy");
			ver2children[1].Sex.Should().Be.EqualTo("Girl");
		}
	}
}