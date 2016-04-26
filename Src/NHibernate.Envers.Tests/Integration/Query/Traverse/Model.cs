using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Query.Traverse
{
	[Audited]
	public class Person
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }
		public virtual int Age { get; set; }
		[Audited(TargetAuditMode = RelationTargetAuditMode.NotAudited)]
		public virtual Address Address { get; set; }
	}

	public class Address
	{
		public virtual int Id { get; set; }
		public virtual string Street { get; set; }
		public virtual int Number { get; set; }
	}

	[Audited]
	public class Car
	{
		public virtual int Id { get; set; }
		public virtual string Make { get; set; }
		public virtual Person Owner { get; set; }
	}
}
