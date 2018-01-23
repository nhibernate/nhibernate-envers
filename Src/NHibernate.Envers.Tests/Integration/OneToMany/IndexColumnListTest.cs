using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Util;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.OneToMany
{
	public class IndexColumnListTest : TestBase
	{
		public IndexColumnListTest(string strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var parent = new IndexColumnListTestParent(1);
			using(var tx = Session.BeginTransaction())
			{
				parent.AddChild(new IndexColumnListTestChild(1, "child1"));
				parent.AddChild(new IndexColumnListTestChild(2, "child2"));
				Session.Save(parent);
				parent.Children.ForEach(x => Session.Save(x));
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				parent.RemoveChild(parent.Children[0]);
				Session.Merge(parent);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				var child = new IndexColumnListTestChild(3, "child3");
				parent.Children.Insert(0, child);
				child.Parent = parent;
				Session.Save(child);
				Session.Merge(parent);
				tx.Commit();
			}
			using(var tx = Session.BeginTransaction())
			{
				while (parent.Children.Any())
				{
					var child = parent.Children[0];
					parent.Children.Remove(child);
					Session.Delete(child);
				}
				Session.Merge(parent);
				tx.Commit();
			}		
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4 }, AuditReader().GetRevisions(typeof(IndexColumnListTestParent), 1));
			CollectionAssert.AreEquivalent(new[] { 1, 2 }, AuditReader().GetRevisions(typeof(IndexColumnListTestChild), 1));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4 }, AuditReader().GetRevisions(typeof(IndexColumnListTestChild), 2));
			CollectionAssert.AreEquivalent(new[] { 3, 4 }, AuditReader().GetRevisions(typeof(IndexColumnListTestChild), 3));
		}

		[Test]
		public void VerifyIndexCollectionRev1()
		{
			var parent = AuditReader().Find<IndexColumnListTestParent>(1, 1);
			parent.Children.Select(x => x.Id).Should().Have.SameSequenceAs(1, 2);
		}

		[Test]
		public void VerifyIndexCollectionRev2()
		{
			var parent = AuditReader().Find<IndexColumnListTestParent>(1, 2);
			parent.Children.Select(x => x.Id).Should().Have.SameSequenceAs(2);
		}
		
		[Test]
		public void VerifyIndexCollectionRev3()
		{
			var parent = AuditReader().Find<IndexColumnListTestParent>(1, 3);
			parent.Children.Select(x => x.Id).Should().Have.SameSequenceAs(3, 2);
		}
		
		[Test]
		public void VerifyIndexCollectionRev4()
		{
			var parent = AuditReader().Find<IndexColumnListTestParent>(1, 4);
			parent.Children.Should().Be.Empty();
		}
		
		protected override IEnumerable<string> Mappings => new[] { "Integration.OneToMany.IndexColumnList.hbm.xml" };
	}
	
	[Audited]
	public class IndexColumnListTestParent
	{
		public IndexColumnListTestParent(int id)
		{
			Id = id;
			Children = new List<IndexColumnListTestChild>();
		}
		public IndexColumnListTestParent()
		{
		}
			
		public virtual int Id { get; protected set; }
		public virtual IList<IndexColumnListTestChild> Children { get; protected set; }

		public virtual void AddChild(IndexColumnListTestChild child)
		{
			if (child != null)
			{
				RemoveChild(child);
			}
			child.Parent = this;
			Children.Add(child);
		}

		public virtual void RemoveChild(IndexColumnListTestChild child)
		{
			var p = child?.Parent;
			if (p == null) return;
			p.Children.Remove(child);
			child.Parent = this;
		}

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

	[Audited]
	public class IndexColumnListTestChild
	{
		public IndexColumnListTestChild(int id, string name)
		{
			Id = id;
			Name = name;
		}
		public IndexColumnListTestChild()
		{	
		}
		
		public virtual int Id { get; protected set; }
		public virtual string Name { get; protected set; }
		public virtual IndexColumnListTestParent Parent { get; set; }
		
		public override bool Equals(object obj)
		{
			var that = obj as IndexColumnListTestChild;
			return that?.Id == Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}