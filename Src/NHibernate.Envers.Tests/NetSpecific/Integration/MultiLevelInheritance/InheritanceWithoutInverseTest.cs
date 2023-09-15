using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.MultiLevelInheritance
{
	public partial class InheritanceWithoutInverseTest : TestBase
	{
		public InheritanceWithoutInverseTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		private InheritedParent parent;

		protected override void Initialize()
		{
			parent = new InheritedParent { Childs = new HashSet<Child>() };
			parent.Childs.Add(new Child { Parent = parent });
		}

		[Test]
		public void ShouldNotThrowExceptionOnCreatingAudit()
		{
			Assert.DoesNotThrow(saveParent);
		}

		[Test]
		public void ShouldAuditParentAndChildProperly()
		{
			saveParent();
			var auditedInheritedParent = AuditReader().CreateQuery().ForRevisionsOf<InheritedParent>().Single();
			var auditedParent = AuditReader().CreateQuery().ForRevisionsOf<Parent>().Single();
			var auditedChild = AuditReader().CreateQuery().ForRevisionsOf<Child>().Single();

			Assert.AreEqual(parent.Id, auditedInheritedParent.Id);
			Assert.AreEqual(parent.Id, auditedParent.Id);
			Assert.AreEqual(parent.Childs.Single().Id, auditedChild.Id);
		}

		private void saveParent()
		{
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(parent);
				tx.Commit();
			}
		}
	}
}
