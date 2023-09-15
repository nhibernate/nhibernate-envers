using System.Collections.Generic;
using NHibernate.Envers.Tests.Integration.ModifiedFlags.Entities.ManyToMany;
using NUnit.Framework;
using SharpTestsEx;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags
{
	public partial class HasChangedAuditedManyToManyRemovalTest : AbstractModifiedFlagsEntityTest
	{
		private int professorId;
		private int studentId;

		public HasChangedAuditedManyToManyRemovalTest(AuditStrategyForTest strategyType) : base(strategyType)
		{
		}

		protected override void Initialize()
		{
			var professor = new Professor();
			var student = new Student();
			//rev 1 - insert
			using (var tx = Session.BeginTransaction())
			{
				professor.Students.Add(student);
				student.Professors.Add(professor);
				professorId = (int) Session.Save(professor);
				studentId = (int) Session.Save(student);
				tx.Commit();
			}
			Session.Clear();
			//rev 2 - delete
			using (var tx = Session.BeginTransaction())
			{
				Session.Delete(Session.Load<Professor>(professorId));
				Session.Delete(Session.Load<Student>(studentId));
				tx.Commit();
			}
		}

		[Test]
		public void ShouldNotThrow()
		{
			AuditReader().GetRevisions(typeof(Professor), professorId).Should().Have.SameSequenceAs(1, 2);
			AuditReader().GetRevisions(typeof(Student), studentId).Should().Have.SameSequenceAs(1, 2);
		}

		protected override IEnumerable<string> Mappings
		{
			get
			{
				return new[] { "Integration.ModifiedFlags.Entities.ManyToMany.Mapping.hbm.xml" };
			}
		}
	}
}