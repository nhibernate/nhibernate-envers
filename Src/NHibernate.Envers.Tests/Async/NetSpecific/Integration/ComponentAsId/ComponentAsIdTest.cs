﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using NUnit.Framework;
using NHibernate.Envers.Query;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.ComponentAsId
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class ComponentAsIdTest : TestBase
	{
		
		[Test]
		public void ComponentAsIdTestMethodAsync()
		{
			Assert.DoesNotThrowAsync(async () =>
			{
				var ent1 = new Entity1
				{
					Id = 1
				};

				await (SaveAsync(ent1)).ConfigureAwait(false);

				var ent2 = new Entity2()
				{
					Id = 1
				};

				await (SaveAsync(ent2)).ConfigureAwait(false);

				var udf = new SomeEntUDF
				{
					Id = new ComponentAsId
					{
						Key1 = ent1,
						Key2 = ent2
					}
				};

				await (SaveAsync(udf)).ConfigureAwait(false);

				await (DelAsync(udf)).ConfigureAwait(false);
				await (DelAsync(ent1)).ConfigureAwait(false);
			});
		}

		[Test]
		public async Task ComponentAsIdGetAuditAsync()
		{
			var ent1 = new Entity1
			{
				Id = 1
			};

			await (SaveAsync(ent1)).ConfigureAwait(false);

			var ent2 = new Entity2()
			{
				Id = 1
			};

			await (SaveAsync(ent2)).ConfigureAwait(false);

			var udf = new SomeEntUDF
			{
				Id = new ComponentAsId
				{
					Key1 = ent1,
					Key2 = ent2
				},
			};

			await (SaveAsync(udf)).ConfigureAwait(false);


			var history = await (Session.Auditer().CreateQuery()
				.ForRevisionsOfEntity(typeof(SomeEntUDF), false, true)
				.Add(AuditEntity.Property("Id.Key1.Id").Eq(ent1.Id))
				.GetResultListAsync()).ConfigureAwait(false);

			Assert.AreEqual(1, history.Count);

		}

		async Task SaveAsync(object o, CancellationToken cancellationToken = default(CancellationToken))
		{
			using (var tran = Session.BeginTransaction())
			{
				await (Session.SaveAsync(o, cancellationToken)).ConfigureAwait(false);
				await (tran.CommitAsync(cancellationToken)).ConfigureAwait(false);
			}
		}

		async Task DelAsync(object o, CancellationToken cancellationToken = default(CancellationToken))
		{
			using (var tran = Session.BeginTransaction())
			{
				await (Session.DeleteAsync(o, cancellationToken)).ConfigureAwait(false);
				await (tran.CommitAsync(cancellationToken)).ConfigureAwait(false);
			}
		}
	}
}
