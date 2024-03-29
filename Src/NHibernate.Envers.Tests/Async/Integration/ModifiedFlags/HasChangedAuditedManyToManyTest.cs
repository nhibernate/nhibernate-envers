﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tests.Integration.EntityNames.ManyToManyAudited;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	using System.Threading.Tasks;
	public partial class HasChangedAuditedManyToManyTest : AbstractModifiedFlagsEntityTest
	{

		[Test]
		public async Task VerifyHasChangedPerson1Async()
		{
			(await (AuditReader().CreateQuery().ForRevisionsOfEntity("Personaje", false, false)
						.Add(AuditEntity.Id().Eq(idPers1))
						.Add(AuditEntity.Property("Cars").HasChanged())
						.GetResultListAsync()).ConfigureAwait(false))
						.ExtractRevisionNumbersFromRevision()
						.Should().Have.SameSequenceAs(1);

			(await (AuditReader().CreateQuery().ForRevisionsOfEntity("Personaje", false, false)
						.Add(AuditEntity.Id().Eq(idPers1))
						.Add(AuditEntity.Property("Cars").HasNotChanged())
						.GetResultListAsync()).ConfigureAwait(false))
						.ExtractRevisionNumbersFromRevision()
						.Should().Have.SameSequenceAs(2);
		}

		[Test]
		public async Task VerifyHasChangedPerson2Async()
		{
			(await (AuditReader().CreateQuery().ForRevisionsOfEntity("Personaje", false, false)
						.Add(AuditEntity.Id().Eq(idPers2))
						.Add(AuditEntity.Property("Cars").HasChanged())
						.GetResultListAsync()).ConfigureAwait(false))
						.ExtractRevisionNumbersFromRevision()
						.Should().Have.SameSequenceAs(1, 2);

			(await (AuditReader().CreateQuery().ForRevisionsOfEntity("Personaje", false, false)
						.Add(AuditEntity.Id().Eq(idPers2))
						.Add(AuditEntity.Property("Cars").HasNotChanged())
						.GetResultListAsync()).ConfigureAwait(false))
						.ExtractRevisionNumbersFromRevision()
						.Should().Be.Empty();
		}

		[Test]
		public async Task VerifyHasChangedCar1Async()
		{
			var list = (await (AuditReader().CreateQuery().ForHistoryOf<Car, DefaultRevisionEntity>(false)
						.Add(AuditEntity.Id().Eq(idCar1))
						.Add(AuditEntity.Property("Owners").HasChanged())
						.ResultsAsync()).ConfigureAwait(false)).ToList();
			list.Should().Have.Count.EqualTo(1);
			list.ExtractRevisionNumbersFromHistory().Should().Have.SameSequenceAs(1);

			var list2 = await (AuditReader().CreateQuery().ForRevisionsOfEntity(typeof(Car), false, false)
						.Add(AuditEntity.Id().Eq(idCar1))
						.Add(AuditEntity.Property("Owners").HasNotChanged())
						.GetResultListAsync()).ConfigureAwait(false);
			list2.Count.Should().Be.EqualTo(0);
		}
	}
}