using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Tests.Entities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.SortedSetAndMap
{
	public partial class SortedSetAndMapTest : TestBase
	{
		private Guid id;

		public SortedSetAndMapTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.Mapping.hbm.xml", "NetSpecific.Integration.SortedSetAndMap.Mapping.hbm.xml" };
			}
		}

		protected override void Initialize()
		{
			var entity = new SortedSetEntity();
			var a = new StrTestEntity {Str = "a"};
			var b = new StrTestEntity {Str = "b"};

			using (var tx = Session.BeginTransaction())
			{
				id = (Guid) Session.Save(entity);
				tx.Commit();
			}

			using (var tx = Session.BeginTransaction())
			{
				entity.SortedSet.Add(a);
				entity.SortedSet.Add(b);
				tx.Commit();
			}

			using (var tx = Session.BeginTransaction())
			{
				entity.SortedSet.Remove(b);
				entity.SortedMap[a] = "a";
				entity.SortedMap[b] = "b";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, AuditReader().GetRevisions(typeof(SortedSetEntity), id));
		}

		[Test]
		public void VerifyHistory1()
		{
			var rev = AuditReader().Find<SortedSetEntity>(id, 1);

			CollectionAssert.IsEmpty(rev.SortedSet);
			CollectionAssert.IsEmpty(rev.SortedMap);
		}

		[Test]
		public void VerifyHistory2()
		{
			var rev = AuditReader().Find<SortedSetEntity>(id, 2);

			rev.SortedSet.Count
				.Should().Be.EqualTo(2);
			rev.SortedSet.First().Str
				.Should().Be.EqualTo("b");
			rev.SortedSet.Last().Str
				.Should().Be.EqualTo("a");

			CollectionAssert.IsEmpty(rev.SortedMap);
		}

		[Test]
		public void VerifyHistory3()
		{
			var rev = AuditReader().Find<SortedSetEntity>(id, 3);

			rev.SortedSet.Count
				.Should().Be.EqualTo(1);
			rev.SortedSet.First().Str
				.Should().Be.EqualTo("a");

			rev.SortedMap.Count
				.Should().Be.EqualTo(2);
			rev.SortedMap.Keys.First().Str
				.Should().Be.EqualTo("b");
			rev.SortedMap.Keys.Last().Str
				.Should().Be.EqualTo("a");
		}

		[Test]
		public void SortedSetShouldStillHaveCorrectComparer()
		{
			var newObj = new StrTestEntity {Str = "c"};
			var rev = AuditReader().Find<SortedSetEntity>(id, 3);

			rev.SortedSet.Count
				.Should().Be.GreaterThan(0);

			rev.SortedSet.Add(newObj);

			rev.SortedSet.First()
				.Should().Be.SameInstanceAs(newObj);
		}

		[Test]
		public void SortedMapShouldStillHaveCorrectComparer()
		{
			var newObj = new StrTestEntity { Str = "c" };
			var rev = AuditReader().Find<SortedSetEntity>(id, 3);

			rev.SortedMap.Count
				.Should().Be.GreaterThan(0);


			rev.SortedMap.Add(newObj, "dd");

			rev.SortedMap.Keys.First()
				.Should().Be.SameInstanceAs(newObj);
		}
	}
}