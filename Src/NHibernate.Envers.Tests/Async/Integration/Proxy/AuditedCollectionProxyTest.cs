﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NHibernate.Envers.Tests.Entities.OneToMany;
using NHibernate.Proxy;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Proxy
{
	using System.Threading.Tasks;
	public partial class AuditedCollectionProxyTest : TestBase
	{

		[Test]
		public async Task VerifyProxyIdentifierAsync()
		{
			var refEntity = await (Session.LoadAsync<SetRefEdEntity>(1)).ConfigureAwait(false);
			refEntity.Should().Be.InstanceOf<INHibernateProxy>();

			var refingEntity3 = new SetRefIngEntity { Id = 3, Data = "refing2", Reference = refEntity };

			//rev 3
			using (var tx = Session.BeginTransaction())
			{
				await (Session.SaveAsync(refingEntity3)).ConfigureAwait(false);
				await (tx.CommitAsync()).ConfigureAwait(false);
			}
		}
	}
}