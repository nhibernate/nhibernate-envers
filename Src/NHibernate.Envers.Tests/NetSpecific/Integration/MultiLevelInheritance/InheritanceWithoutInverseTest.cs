using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.MultiLevelInheritance
{
	public class InheritanceWithoutInverseTest : TestBase
	{
		public InheritanceWithoutInverseTest(string strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
		}

		[Test]
		public void Test()
		{
			var parent = new InheritedParent {Childs = new HashSet<Child>()};
			parent.Childs.Add(new Child { Parent = parent });

			using (var tx = Session.BeginTransaction())
			{
				Session.Save(parent);
				Assert.DoesNotThrow(() => tx.Commit());
			}
		}
	}
}
