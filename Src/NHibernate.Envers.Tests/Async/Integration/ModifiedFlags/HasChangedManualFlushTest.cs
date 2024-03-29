﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using NHibernate.Envers.Tests.Integration.Basic;
using NHibernate.Envers.Tests.Tools;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	using System.Threading.Tasks;
	public partial class HasChangedManualFlushTest : AbstractModifiedFlagsEntityTest
	{

		[Test]
		public async Task ShouldHaveChangedOnDoubleFlushAsync()
		{
			var list = await (QueryForPropertyHasChangedAsync(typeof (BasicTestEntity1), id, "Str1")).ConfigureAwait(false);
			list.Count.Should().Be.EqualTo(2);
			list.ExtractRevisionNumbersFromRevision().Should().Have.SameSequenceAs(1, 2);

			list = await (QueryForPropertyHasChangedAsync(typeof (BasicTestEntity1), id, "Long1")).ConfigureAwait(false);
			list.Count.Should().Be.EqualTo(2);
			list.ExtractRevisionNumbersFromRevision().Should().Have.SameSequenceAs(1, 2);
		}
	}
}