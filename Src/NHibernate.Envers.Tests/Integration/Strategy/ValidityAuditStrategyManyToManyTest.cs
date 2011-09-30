using System.Collections.Generic;
using Iesi.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Strategy;
using NHibernate.Envers.Tests.Entities.ManyToMany;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Strategy
{
	[TestFixture]
	public class ValidityAuditStrategyManyToManyTest : TestBase
	{
		private int ingId = 1;
		private int edId = 2;

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
				owningEntity.References = new HashedSet<SetOwnedEntity> {ownedEntity};
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
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4, 5 }, AuditReader().GetRevisions<SetOwningEntity>(ingId));
			CollectionAssert.AreEquivalent(new[] { 1, 2, 3, 4, 5 }, AuditReader().GetRevisions<SetOwnedEntity>(edId));
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

		}
/*

	@Test
	public void testHistoryOfIng1() {
		SetOwningEntity ver_empty = createOwningEntity();
		SetOwningEntity ver_child = createOwningEntity(new SetOwnedEntity(ed_id, "child"));

		  assertEquals(getAuditReader().find(SetOwningEntity.class, ing_id, 1), ver_empty);
		  assertEquals(getAuditReader().find(SetOwningEntity.class, ing_id, 2), ver_child);
		  assertEquals(getAuditReader().find(SetOwningEntity.class, ing_id, 3), ver_empty);
		  assertEquals(getAuditReader().find(SetOwningEntity.class, ing_id, 4), ver_child);
		  assertEquals(getAuditReader().find(SetOwningEntity.class, ing_id, 5), ver_empty);
	}

	 @Test
	public void testHistoryOfEd1() {
		SetOwnedEntity ver_empty = createOwnedEntity();
		SetOwnedEntity ver_child = createOwnedEntity(new SetOwningEntity(ing_id, "parent"));

		  assertEquals(getAuditReader().find(SetOwnedEntity.class, ed_id, 1), ver_empty);
		  assertEquals(getAuditReader().find(SetOwnedEntity.class, ed_id, 2), ver_child);
		  assertEquals(getAuditReader().find(SetOwnedEntity.class, ed_id, 3), ver_empty);
		  assertEquals(getAuditReader().find(SetOwnedEntity.class, ed_id, 4), ver_child);
		  assertEquals(getAuditReader().find(SetOwnedEntity.class, ed_id, 5), ver_empty);
	}

	 private SetOwningEntity createOwningEntity(SetOwnedEntity... owned) {
		  SetOwningEntity result = new SetOwningEntity(ing_id, "parent");
		  result.setReferences(new HashSet<SetOwnedEntity>());
		  for (SetOwnedEntity setOwnedEntity : owned) {
				result.getReferences().add(setOwnedEntity);
		  }

		  return result;
	 }

	 private SetOwnedEntity createOwnedEntity(SetOwningEntity... owning) {
		  SetOwnedEntity result = new SetOwnedEntity(ed_id, "child");
		  result.setReferencing(new HashSet<SetOwningEntity>());
		  for (SetOwningEntity setOwningEntity : owning) {
				result.getReferencing().add(setOwningEntity);
		  }

		  return result;
	 }
		 * 
		 * 
		 * 
		 */

		protected override void AddToConfiguration(Cfg.Configuration configuration)
		{
			configuration.SetProperty(ConfigurationKey.AuditStrategy, typeof(ValidityAuditStrategy).AssemblyQualifiedName);
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