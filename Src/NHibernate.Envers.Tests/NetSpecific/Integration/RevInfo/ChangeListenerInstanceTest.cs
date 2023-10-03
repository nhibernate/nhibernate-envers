using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.RevInfo
{
	public partial class ChangeListenerInstanceTest : TestBase
	{
		private testListener listener;

		public ChangeListenerInstanceTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml", "Entities.RevEntity.ExceptionListenerRevEntity.hbm.xml" };
			}
		}

		protected override IMetaDataProvider EnversConfiguration()
		{
			listener = new testListener();
			return new AttributeConfigurationWithRevisionListener(listener);
		}

		protected override void Initialize()
		{
			var te = new StrTestEntity { Str = "x" };
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(te);
				listener.Called.Should().Be.False();
				tx.Commit();
				listener.Called.Should().Be.True();
			}
		}

		[Test]
		public void EntityShouldHaveBeenPersisted()
		{
			Session.CreateQuery("select count(s) from StrTestEntity s where s.Str='x'").UniqueResult<long>()
				.Should().Be.EqualTo(1);
		}

		private class testListener : IRevisionListener
		{
			public void NewRevision(object revisionEntity)
			{
				Called = true;
			}

			public bool Called { get; private set; }
		}
	}
}