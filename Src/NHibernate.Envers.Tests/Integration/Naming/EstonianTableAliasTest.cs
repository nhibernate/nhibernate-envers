using System.Collections.Generic;
using ee.Estonia.Entities;
using NHibernate.Envers.Configuration.Attributes;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Naming
{
	public partial class EstonianTableAliasTest : TestBase
	{
		private long parentId;
		private long childId;

		public EstonianTableAliasTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			//Revision 1
			using (var tx = Session.BeginTransaction())
			{
				var parent = new Parent {Data = "data", Collection = new HashSet<Child>()};
				var child = new Child {Data = "child"};
				parent.Collection.Add(child);
				childId = (long) Session.Save(child);
				parentId = (long) Session.Save(parent);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyAuditChildTableAlias()
		{
			var parent = new Parent { Data = "data", Id = parentId };
			var child = new Child { Data = "child", Id = childId };

			var ver1 = AuditReader().Find<Parent>(parentId, 1);

			ver1.Should().Be.EqualTo(parent);
			ver1.Collection.Should().Have.SameValuesAs(child);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[]{"Integration.Naming.Estonia.hbm.xml"};
			}
		}
	}
}

namespace ee.Estonia.Entities
{
	[Audited]
	public class Child
	{
		public virtual long Id { get; set; }
		public virtual string Data { get; set; }

		public override bool Equals(object obj)
		{
			var that = obj as Child;
			if (that == null)
				return false;
			if (Data != null ? !Data.Equals(that.Data) : that.Data != null) return false;
			return Id == that.Id;
		}

		public override int GetHashCode()
		{
			var result = Id.GetHashCode();
			result = 31 * result + (Data != null ? Data.GetHashCode() : 0);
			return result;
		}
	}

	[Audited]
	public class Parent
	{
		public virtual long Id { get; set; }
		public virtual string Data { get; set; }
		public virtual ISet<Child> Collection { get; set; }

		public override bool Equals(object obj)
		{
			var that = obj as Parent;
			if (that == null)
				return false;
			if (Data != null ? !Data.Equals(that.Data) : that.Data != null) return false;
			return Id == that.Id;
		}

		public override int GetHashCode()
		{
			var result = Id.GetHashCode();
			result = 31 * result + (Data != null ? Data.GetHashCode() : 0);
			return result;
		}
	}
}