using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Collection
{
	public class IndexColumnListTest :TestBase
	{
		public IndexColumnListTest(string strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var parent = new IndexColumnListTestParent(1);
			using(var tx = Session.BeginTransaction())
			{
				parent.Children.Add("child1");
				parent.Children.Add("child2");
				Session.Save(parent);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				parent.Children.RemoveAt(0);
				Session.Merge(parent);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				parent.Children.Insert(0, "child3");
				Session.Merge(parent);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				parent.Children.Clear();
				Session.Merge(parent);
				tx.Commit();
			}		
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4 }, AuditReader().GetRevisions(typeof(IndexColumnListTestParent), 1));
		}

		[Test]
		public void VerifyIndexCollectionRev1()
		{
			var parent = AuditReader().Find<IndexColumnListTestParent>(1, 1);
			parent.Children.Should().Have.SameSequenceAs("child1", "child2");
		}

		[Test]
		public void VerifyIndexCollectionRev2()
		{
			var parent = AuditReader().Find<IndexColumnListTestParent>(1, 2);
			parent.Children.Should().Have.SameSequenceAs("child2");
		}
		
		[Test]
		public void VerifyIndexCollectionRev3()
		{
			var parent = AuditReader().Find<IndexColumnListTestParent>(1, 3);
			parent.Children.Should().Have.SameSequenceAs("child3", "child2");
		}
		
		[Test]
		public void VerifyIndexCollectionRev4()
		{
			var parent = AuditReader().Find<IndexColumnListTestParent>(1, 4);
			parent.Children.Should().Be.Empty();
		}

		protected override IEnumerable<string> Mappings => new[] { "Integration.Collection.IndexColumnList.hbm.xml" };
	}
	
	[Audited]
	public class IndexColumnListTestParent
	{
		public IndexColumnListTestParent(int id)
		{
			Id = id;
			Children = new List<string>();
		}
		public IndexColumnListTestParent()
		{
		}
			
		public virtual int Id { get; protected set; }
		public virtual IList<string> Children { get; protected set; }

		public override bool Equals(object obj)
		{
			var that = obj as IndexColumnListTestParent;
			return that?.Id == Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}