﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Linq;
using NHibernate.Mapping;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Single.DiscriminatorFormula
{
	using System.Threading.Tasks;
	public partial class DiscriminatorFormulaTest : TestBase
	{

		[Test]
		public async Task VerifyRevisionCountsAsync()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 3 }, await (AuditReader().GetRevisionsAsync(typeof(ChildEntity),childVer1.Id)).ConfigureAwait(false));
			CollectionAssert.AreEquivalent(new[] { 2, 4 }, await (AuditReader().GetRevisionsAsync(typeof(ParentEntity),parentVer1.Id)).ConfigureAwait(false));
		}

		[Test]
		public async Task VerifyHistoryOfParentAsync()
		{
			(await (AuditReader().FindAsync<ParentEntity>(parentVer1.Id, 2)).ConfigureAwait(false))
				.Should().Be.EqualTo(parentVer1);
			(await (AuditReader().FindAsync<ParentEntity>(parentVer2.Id, 4)).ConfigureAwait(false))
				.Should().Be.EqualTo(parentVer2);
		}

		[Test]
		public async Task VerifyHistoryOfChildAsync()
		{
			(await (AuditReader().FindAsync<ChildEntity>(childVer1.Id, 1)).ConfigureAwait(false))
				.Should().Be.EqualTo(childVer1);
			(await (AuditReader().FindAsync<ChildEntity>(childVer2.Id, 3)).ConfigureAwait(false))
				.Should().Be.EqualTo(childVer2);
		}

		[Test]
		public async Task PolymorphicQueryAsync()
		{
			(await (AuditReader().CreateQuery().ForEntitiesAtRevision<ChildEntity>(1).ResultsAsync()).ConfigureAwait(false)).First()
				.Should().Be.EqualTo(childVer1);
			(await (AuditReader().CreateQuery().ForEntitiesAtRevision<ParentEntity>(1).ResultsAsync()).ConfigureAwait(false)).First()
				.Should().Be.EqualTo(childVer1);

			(await (AuditReader().CreateQuery().ForRevisionsOfEntity(typeof (ChildEntity), true, false).GetResultListAsync<ChildEntity>()).ConfigureAwait(false))
				.Should().Have.SameValuesAs(childVer1, childVer2);
			(await (AuditReader().CreateQuery().ForRevisionsOfEntity(typeof(ParentEntity), true, false).GetResultListAsync<ParentEntity>()).ConfigureAwait(false))
				.Should().Have.SameValuesAs(childVer1, childVer2, parentVer1, parentVer2);
		}
	}
}