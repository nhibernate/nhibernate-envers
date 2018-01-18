using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Tests.NetSpecific.Integration.BidirectionalList.DifferentAccessTest;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Async
{
	public class AuditEventListenerAsyncTest : TestBase
	{
		public AuditEventListenerAsyncTest(string strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
		}

		[Test]
		public async void AsyncImplementationOfAuditEventListenerWorks()
		{
			var parent = new Parent();
			var child = new Child { Parent = parent };
			using (var tx = Session.BeginTransaction())
			{
				await Session.SaveAsync(parent);
				await tx.CommitAsync();
			}
			using (var tx = Session.BeginTransaction())
			{
				parent.Children.Add(child);
				await tx.CommitAsync();
			}
			using (var tx = Session.BeginTransaction())
			{
				parent.Name = Guid.NewGuid().ToString();
				await tx.CommitAsync();
			}
			using (var tx = Session.BeginTransaction())
			{
				await Session.DeleteAsync(parent);
				await tx.CommitAsync();
			}
			AuditReader().GetRevisions(typeof(Parent), parent.Id).Count()
				.Should().Be.EqualTo(4);
		}

		protected override IEnumerable<string> Mappings { get; } = new[]{ "NetSpecific.Integration.BidirectionalList.DifferentAccessTest.Mapping.hbm.xml" };
	}
}