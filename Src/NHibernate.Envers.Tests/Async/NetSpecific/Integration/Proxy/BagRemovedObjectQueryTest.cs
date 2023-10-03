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
using NHibernate.Cfg;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.NetSpecific.Entities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Proxy
{
	using System.Threading.Tasks;
	public partial class BagRemovedObjectQueryTest : TestBase
	{

		[Test]
		public async Task VerifyHistoryOfParentAsync()
		{
			(await (AuditReader().FindAsync<BagParent>(parentId, 1)).ConfigureAwait(false))
						 .Children.Single().Name.Should().Be.EqualTo("child1");
			(await (AuditReader().FindAsync<BagParent>(parentId, 2)).ConfigureAwait(false))
						 .Children.Single().Name.Should().Be.EqualTo("child12");
			var rev3 = await (AuditReader().CreateQuery().ForRevisionsOfEntity(typeof(BagParent), true, true)
			             .Add(AuditEntity.Id().Eq(parentId))
			             .Add(AuditEntity.RevisionNumber().Eq(3))
			             .GetSingleResultAsync<BagParent>()).ConfigureAwait(false);
			rev3.Children.Single().Name.Should().Be.EqualTo("child12");
		}
	}
}