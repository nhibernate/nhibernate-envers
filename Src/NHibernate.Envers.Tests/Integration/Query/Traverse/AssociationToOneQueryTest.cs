using System.Linq;
using NHibernate.Envers.Query;
using NHibernate.SqlCommand;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Query.Traverse
{
	public partial class AssociationToOneQueryTest : TestBase
	{
		private int fordId;
		private int toyotaId;
		private int vwId;
		private int fordOwnerId;
		private int toyotaOwnerId;
		private int vwOwnerId;
		private int addressId1;
		private int addressId2;

		public AssociationToOneQueryTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var address1 = new Address { Street = "Sveavägen", Number = 5 };
			var address2 = new Address { Street = "Skagis", Number = 20 };
			var toyotaOwner = new Person { Name = "Toyota Owner", Address = address2, Age = 30 };
			var vWOwner = new Person { Name = "VW Owner", Address = address1, Age = 20 };
			var fordOwner = new Person { Name = "Ford Owner", Address = address1, Age = 30 };
			var nonOwner = new Person { Name = "Non Owner", Address = address1, Age = 30 };
			var vw = new Car { Make = "VW", Owner = vWOwner };
			var ford = new Car { Make = "Ford", Owner = fordOwner };
			var toyota = new Car { Make = "Toyota", Owner = toyotaOwner };
			using (var tx = Session.BeginTransaction())
			{
				addressId1 = (int) Session.Save(address1);
				addressId2 = (int) Session.Save(address2);
				vwOwnerId = (int) Session.Save(vWOwner);
				fordOwnerId = (int) Session.Save(fordOwner);
				toyotaOwnerId = (int) Session.Save(toyotaOwner);
				Session.Save(nonOwner);
				vwId = (int) Session.Save(vw);
				fordId = (int) Session.Save(ford);
				toyotaId = (int) Session.Save(toyota);
				tx.Commit();
			}
			using (var tx = Session.BeginTransaction())
			{
				toyotaOwner.Age = 40;
				tx.Commit();
			}
		}


		[Test]
		public void ShouldTraverse()
		{
			AuditReader().CreateQuery().ForEntitiesAtRevision<Car>(1)
				.TraverseRelation("Owner", JoinType.InnerJoin)
				.Add(AuditEntity.Property("Name").Like("Ford%")).Single()
				.Id.Should().Be.EqualTo(fordId);
		}

		[Test]
		public void ShouldDoMultipleTraverse()
		{
			AuditReader().CreateQuery().ForEntitiesAtRevision<Car>(1)
				.TraverseRelation("Owner", JoinType.InnerJoin)
				.TraverseRelation("Address", JoinType.InnerJoin)
				.Add(AuditEntity.Property("Number").Eq(20)).Single()
				.Id.Should().Be.EqualTo(toyotaId);
		}

		[Test]
		public void ShouldTraverseWithUpList()
		{
			AuditReader().CreateQuery().ForEntitiesAtRevision<Car>(1)
				.TraverseRelation("Owner", JoinType.InnerJoin)
				.Add(AuditEntity.Property("Age").Ge(30))
				.Add(AuditEntity.Property("Age").Lt(40))
				.Up()
				.AddOrder(AuditEntity.Property("Make").Asc())
				.Results().Select(x => x.Id)
				.Should().Have.SameSequenceAs(fordId, toyotaId);
		}

		[Test]
		public void ShouldTraverseWithUp()
		{
			AuditReader().CreateQuery().ForEntitiesAtRevision<Car>(2)
				.TraverseRelation("Owner", JoinType.InnerJoin)
				.Add(AuditEntity.Property("Age").Ge(30))
				.Add(AuditEntity.Property("Age").Lt(40))
				.Up()
				.AddOrder(AuditEntity.Property("Make").Asc()).Single()
				.Id.Should().Be.EqualTo(fordId);
		}

		[Test]
		public void ShouldOrderOnUpAscending()
		{
			AuditReader().CreateQuery().ForEntitiesAtRevision<Car>(1)
				.TraverseRelation("Owner", JoinType.InnerJoin)
				.TraverseRelation("Address", JoinType.InnerJoin)
				.AddOrder(AuditEntity.Property("Number").Asc())
				.Up()
				.AddOrder(AuditEntity.Property("Age").Asc())
				.Results().Select(x => x.Id)
				.Should().Have.SameSequenceAs(vwId, fordId, toyotaId);
		}

		[Test]
		public void ShouldOrderOnUpDescending()
		{
			AuditReader().CreateQuery().ForEntitiesAtRevision<Car>(1)
				.TraverseRelation("Owner", JoinType.InnerJoin)
				.TraverseRelation("Address", JoinType.InnerJoin)
				.AddOrder(AuditEntity.Property("Number").Asc())
				.Up()
				.AddOrder(AuditEntity.Property("Age").Desc())
				.Results().Select(x => x.Id)
				.Should().Have.SameSequenceAs(fordId, vwId, toyotaId);
		}

		[Test]
		public void ShouldDoProjection()
		{
			AuditReader().CreateQuery().ForEntitiesAtRevision(typeof (Car), 2)
				.TraverseRelation("Owner", JoinType.InnerJoin)
				.AddProjection(AuditEntity.Property("Age"))
				.AddOrder(AuditEntity.Property("Age").Asc())
				.GetResultList<int>()
				.Should().Have.SameSequenceAs(20, 30, 40);
		}

		[Test]
		public void ShouldDoProjectionOnTraversedPropertyNotDistinct()
		{
			AuditReader().CreateQuery().ForEntitiesAtRevision(typeof(Car), 2)
				.TraverseRelation("Owner", JoinType.InnerJoin)
				.AddOrder(AuditEntity.Property("Age").Asc())
				.TraverseRelation("Address", JoinType.InnerJoin)
				.AddProjection(AuditEntity.SelectEntity(false))
				.GetResultList<Address>().Select(x => x.Id)
				.Should().Have.SameSequenceAs(addressId1, addressId1, addressId2);
		}

		[Test]
		public void ShouldDoProjectionOnTraversedPropertyDistinct()
		{
			AuditReader().CreateQuery().ForEntitiesAtRevision(typeof(Car), 2)
				.TraverseRelation("Owner", JoinType.InnerJoin)
				.TraverseRelation("Address", JoinType.InnerJoin)
				.AddProjection(AuditEntity.SelectEntity(true))
				.AddOrder(AuditEntity.Property("Number").Asc())
				.GetResultList<Address>().Select(x => x.Id)
				.Should().Have.SameSequenceAs(addressId1, addressId2);
		}

		[Test]
		public void ShouldDoMultipleProjections()
		{
			var result = AuditReader().CreateQuery().ForEntitiesAtRevision(typeof(Car), 2)
				.TraverseRelation("Owner", JoinType.InnerJoin)
				.AddOrder(AuditEntity.Property("Age").Asc())
				.AddProjection(AuditEntity.SelectEntity(false))
				.TraverseRelation("Address", JoinType.InnerJoin)
				.AddProjection(AuditEntity.Property("Number"))
				.GetResultList();

			result.Count.Should().Be.EqualTo(3);

			var index0 = (object[]) result[0];
			((Person) index0[0]).Id.Should().Be.EqualTo(vwOwnerId);
			index0[1].Should().Be.EqualTo(5);

			var index1 = (object[])result[1];
			((Person)index1[0]).Id.Should().Be.EqualTo(fordOwnerId);
			index1[1].Should().Be.EqualTo(5);

			var index2 = (object[])result[2];
			((Person)index2[0]).Id.Should().Be.EqualTo(toyotaOwnerId);
			index2[1].Should().Be.EqualTo(20);
		}
	}
}