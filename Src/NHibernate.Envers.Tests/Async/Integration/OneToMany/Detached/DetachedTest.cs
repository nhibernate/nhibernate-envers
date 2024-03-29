﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.OneToMany.Detached;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.OneToMany.Detached
{
	using System.Threading.Tasks;
	public partial class DetachedTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountsAsync()
		{
			(await (AuditReader().GetRevisionsAsync(typeof (ListRefCollEntity), parentId)).ConfigureAwait(false)).Should().Have.SameSequenceAs(1, 2);
			(await (AuditReader().GetRevisionsAsync(typeof (StrTestEntity), childId)).ConfigureAwait(false)).Should().Have.SameSequenceAs(1);
		}

		[Test]
		public async Task VerifyHistoryOfParentAsync()
		{
			var parent = new ListRefCollEntity
			             	{
			             		Id = parentId,
			             		Data = "initial data",
			             		Collection = new List<StrTestEntity> {new StrTestEntity {Id = childId, Str = "data"}}
			             	};
			
			var ver1 = await (AuditReader().FindAsync<ListRefCollEntity>(parentId, 1)).ConfigureAwait(false);
			ver1.Should().Be.EqualTo(parent);
			ver1.Collection.Should().Have.SameValuesAs(parent.Collection);

			parent.Data = "modified data";
			var ver2 = await (AuditReader().FindAsync<ListRefCollEntity>(parentId, 2)).ConfigureAwait(false);
			ver2.Should().Be.EqualTo(parent);
			ver1.Collection.Should().Have.SameValuesAs(parent.Collection);
		}
	}
}