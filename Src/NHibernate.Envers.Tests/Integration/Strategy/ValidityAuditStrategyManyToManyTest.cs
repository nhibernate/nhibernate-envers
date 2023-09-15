using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities.ManyToMany;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Strategy
{
	public partial class ValidityAuditStrategyManyToManyTest : ValidityTestBase
	{
		private const int ingId = 1;
		private const int edId = 2;

		protected override void Initialize()
		{
			var owningEntity = new SetOwningEntity {Id = ingId, Data = "parent"};
			var ownedEntity = new SetOwnedEntity {Id = edId, Data = "child"};

			// Revision 1: Initial persist
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(owningEntity);
				Session.Save(ownedEntity);
				tx.Commit();
			}
			// Revision 2: add child for first time
			using (var tx = Session.BeginTransaction())
			{
				owningEntity.References = new HashSet<SetOwnedEntity> {ownedEntity};
				tx.Commit();
			}
			// Revision 3: remove child
			using (var tx = Session.BeginTransaction())
			{
				owningEntity.References.Remove(ownedEntity);
				tx.Commit();
			}
			// Revision 4: add child again
			using (var tx = Session.BeginTransaction())
			{
				owningEntity.References.Add(ownedEntity);
				tx.Commit();
			}
			// Revision 5: remove child again
			using (var tx = Session.BeginTransaction())
			{
				owningEntity.References.Remove(ownedEntity);
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCount()
		{
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4, 5 }, AuditReader().GetRevisions(typeof(SetOwningEntity),ingId));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4, 5 }, AuditReader().GetRevisions(typeof(SetOwnedEntity),edId));
		}

		[Test]
		public void VerifyHistoryOfIng()
		{
			var ed = new SetOwnedEntity {Id = edId, Data = "child"};

			AuditReader().Find<SetOwningEntity>(ingId, 1).References.Should().Be.Empty();
			AuditReader().Find<SetOwningEntity>(ingId, 2).References.Should().Have.SameSequenceAs(ed);
			AuditReader().Find<SetOwningEntity>(ingId, 3).References.Should().Be.Empty();
			AuditReader().Find<SetOwningEntity>(ingId, 4).References.Should().Have.SameSequenceAs(ed);
			AuditReader().Find<SetOwningEntity>(ingId, 5).References.Should().Be.Empty();

		}

		[Test]
		public void VerifyHistoryOfEd()
		{
			var ing = new SetOwningEntity { Id = ingId, Data = "parent" };

			AuditReader().Find<SetOwnedEntity>(edId, 1).Referencing.Should().Be.Empty();
			AuditReader().Find<SetOwnedEntity>(edId, 2).Referencing.Should().Have.SameSequenceAs(ing);
			AuditReader().Find<SetOwnedEntity>(edId, 3).Referencing.Should().Be.Empty();
			AuditReader().Find<SetOwnedEntity>(edId, 4).Referencing.Should().Have.SameSequenceAs(ing);
			AuditReader().Find<SetOwnedEntity>(edId, 5).Referencing.Should().Be.Empty();
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Entities.ManyToMany.Mapping.hbm.xml" };
			}
		}
	}
}