﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.Collection;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	using System.Threading.Tasks;
	public partial class HasChangedStringMapTest : AbstractModifiedFlagsEntityTest
	{

		[Test]
		public async Task VerifyHasChangedAsync()
		{
			(await (QueryForPropertyHasChangedAsync(typeof (StringMapEntity), sme1_id, "Strings")).ConfigureAwait(false))
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1, 2, 3);
			(await (QueryForPropertyHasChangedAsync(typeof(StringMapEntity), sme2_id, "Strings")).ConfigureAwait(false))
				.ExtractRevisionNumbersFromRevision()
				.Should().Have.SameSequenceAs(1, 3);
			(await (QueryForPropertyHasNotChangedAsync(typeof (StringMapEntity), sme1_id, "Strings")).ConfigureAwait(false))
				.ExtractRevisionNumbersFromRevision()
				.Should().Be.Empty();
			(await (QueryForPropertyHasNotChangedAsync(typeof (StringMapEntity), sme2_id, "Strings")).ConfigureAwait(false))
				.ExtractRevisionNumbersFromRevision()
				.Should().Be.Empty(); // in rev 2 there was no version generated for sme2_id
		}
	}
}