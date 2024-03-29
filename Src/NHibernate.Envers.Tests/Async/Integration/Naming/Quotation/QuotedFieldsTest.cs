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

namespace NHibernate.Envers.Tests.Integration.Naming.Quotation
{
	using System.Threading.Tasks;
	public partial class QuotedFieldsTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionsCountsAsync()
		{
			(await (AuditReader().GetRevisionsAsync(typeof(QuotedFieldsEntity), qfeId1)).ConfigureAwait(false))
				.Should().Have.SameSequenceAs(1, 2);
			(await (AuditReader().GetRevisionsAsync(typeof(QuotedFieldsEntity), qfeId2)).ConfigureAwait(false))
				.Should().Have.SameSequenceAs(1, 3);
		}

		[Test]
		public async Task VerifyHistoryOf1Async()
		{
			var ver1 = new QuotedFieldsEntity { Id = qfeId1, Data1 = "data1", Data2 = 1 };
			var ver2 = new QuotedFieldsEntity { Id = qfeId1, Data1 = "data changed", Data2 = 1 };

			(await (AuditReader().FindAsync<QuotedFieldsEntity>(qfeId1, 1)).ConfigureAwait(false)).Should().Be.EqualTo(ver1);
			(await (AuditReader().FindAsync<QuotedFieldsEntity>(qfeId1, 2)).ConfigureAwait(false)).Should().Be.EqualTo(ver2);
			(await (AuditReader().FindAsync<QuotedFieldsEntity>(qfeId1, 3)).ConfigureAwait(false)).Should().Be.EqualTo(ver2);
		}

		[Test]
		public async Task VerifyHistoryOf2Async()
		{
			var ver1 = new QuotedFieldsEntity { Id = qfeId2, Data1 = "data2", Data2 = 2 };
			var ver2 = new QuotedFieldsEntity { Id = qfeId2, Data1 = "data2", Data2 = 3 };

			(await (AuditReader().FindAsync<QuotedFieldsEntity>(qfeId2, 1)).ConfigureAwait(false)).Should().Be.EqualTo(ver1);
			(await (AuditReader().FindAsync<QuotedFieldsEntity>(qfeId2, 2)).ConfigureAwait(false)).Should().Be.EqualTo(ver1);
			(await (AuditReader().FindAsync<QuotedFieldsEntity>(qfeId2, 3)).ConfigureAwait(false)).Should().Be.EqualTo(ver2);
		}
	}
}