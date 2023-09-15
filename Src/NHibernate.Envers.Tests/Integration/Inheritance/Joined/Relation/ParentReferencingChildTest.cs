using NHibernate.Envers.Tests.Integration.Inheritance.Entities;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Joined.Relation
{
	public partial class ParentReferencingChildTest : TestBase
	{
		private Person expLukaszRev1;
		private Person expLukaszRev2;
		private Person expAdamRev4;
		private Role expDirectorRev3;
		private Role expAdminRev2;
		private Role expAdminRev1;

		public ParentReferencingChildTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var lukasz = new Person {Name = "lukasz", Group="IT"};
			var admin = new Role {Name = "Admin", Group = "Confidential"};
			var director = new Role { Name = "Director" };
			var adam = new Person { Name = "Adam", Group = "CEO" };

			//rev 1
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(lukasz);
				lukasz.Roles.Add(admin);
				admin.Members.Add(lukasz);
				Session.Save(admin);
				tx.Commit();
			}
			expAdminRev1 = new Role { Name = "Admin", Group = "Confidential", Id = admin.Id };
			expLukaszRev1 = new Person { Name = "lukasz", Group = "IT", Id = lukasz.Id };
			//rev 2
			using (var tx = Session.BeginTransaction())
			{
				lukasz.Group = "Senior IT";
				lukasz.Name = "Lukasz Antoniak";
				admin.Group = "Very confidential";
				tx.Commit();
			}
			expAdminRev2 = new Role { Name = "Admin", Group = "Very confidential", Id = admin.Id };
			expLukaszRev2 = new Person { Name = "Lukasz Antoniak", Group = "Senior IT", Id = lukasz.Id };
			//rev 3
			using (var tx = Session.BeginTransaction())
			{
				director.Members.Add(lukasz);
				Session.Save(director);
				lukasz.Roles.Add(director);
				tx.Commit();
			}	
			expDirectorRev3 = new Role { Name = "Director", Id = director.Id };
			//rev 4
			using (var tx = Session.BeginTransaction())
			{
				Session.Save(adam);
				director.Members.Add(adam);
				adam.Roles.Add(director);
				tx.Commit();
			}
			expAdamRev4 = new Person { Name = "Adam", Group = "CEO", Id = adam.Id };
			//rev 5
			using (var tx = Session.BeginTransaction())
			{
				admin.Members.Add(adam);
				tx.Commit();
			}			
			//rev 6
			using (var tx = Session.BeginTransaction())
			{
				adam.Name = "Adam Warski";
				tx.Commit();
			}
		}

		[Test]
		public void VerifyRevisionCounts()
		{
			AuditReader().GetRevisions(typeof(Person),expLukaszRev1.Id).Should().Have.SameSequenceAs(1, 2, 3);
			AuditReader().GetRevisions(typeof(RightsSubject),expLukaszRev1.Id).Should().Have.SameSequenceAs(1, 2, 3);

			AuditReader().GetRevisions(typeof(Person),expAdamRev4.Id).Should().Have.SameSequenceAs(4, 5, 6);
			AuditReader().GetRevisions(typeof(RightsSubject),expAdamRev4.Id).Should().Have.SameSequenceAs(4, 5, 6);

			AuditReader().GetRevisions(typeof(Role),expAdminRev1.Id).Should().Have.SameSequenceAs(1, 2, 5);
			AuditReader().GetRevisions(typeof(RightsSubject),expAdminRev1.Id).Should().Have.SameSequenceAs(1, 2, 5);

			AuditReader().GetRevisions(typeof(Role),expDirectorRev3.Id).Should().Have.SameSequenceAs(3, 4);
			AuditReader().GetRevisions(typeof(RightsSubject),expDirectorRev3.Id).Should().Have.SameSequenceAs(3, 4);
		}

		[Test]
		public void VerifyHistoryOfAdam()
		{
			var adamRev4 = AuditReader().Find<Person>(expAdamRev4.Id, 4);
			var rightsSubject5 = AuditReader().Find<RightsSubject>(expAdamRev4.Id, 5);

			adamRev4.Should().Be.EqualTo(expAdamRev4);
			adamRev4.Roles.Should().Have.SameValuesAs(expDirectorRev3);
			rightsSubject5.Roles.Should().Have.SameValuesAs(expDirectorRev3, expAdminRev2);
		}

		[Test]
		public void VerifyHistoryOfLukasz()
		{
			var lukaszRev1 = AuditReader().Find<Person>(expLukaszRev1.Id, 1);
			var lukaszRev2 = AuditReader().Find<Person>(expLukaszRev1.Id, 2);
			var rightsSubject3 = AuditReader().Find<RightsSubject>(expLukaszRev1.Id, 3);
			var lukaszRev3 = AuditReader().Find<Person>(expLukaszRev1.Id, 3);

			lukaszRev1.Should().Be.EqualTo(expLukaszRev1);
			lukaszRev2.Should().Be.EqualTo(expLukaszRev2);
			lukaszRev1.Roles.Should().Have.SameValuesAs(expAdminRev1);
			rightsSubject3.Roles.Should().Have.SameValuesAs(expAdminRev2, expDirectorRev3);
			lukaszRev3.Roles.Should().Have.SameValuesAs(expAdminRev2, expDirectorRev3);
		}

		[Test]
		public void VerifyHistoryOfAdmin()
		{
			var adminRev1 = AuditReader().Find<Role>(expAdminRev1.Id, 1);
			var adminRev2 = AuditReader().Find<Role>(expAdminRev1.Id, 2);
			var adminRev5 = AuditReader().Find<Role>(expAdminRev1.Id, 5);

			adminRev1.Should().Be.EqualTo(expAdminRev1);
			adminRev2.Should().Be.EqualTo(expAdminRev2);
			adminRev1.Members.Should().Have.SameValuesAs(expLukaszRev1);
			adminRev5.Members.Should().Have.SameValuesAs(expLukaszRev2, expAdamRev4);
		}
	}
}