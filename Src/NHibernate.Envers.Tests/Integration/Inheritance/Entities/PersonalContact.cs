namespace NHibernate.Envers.Tests.Integration.Inheritance.Entities
{
	[Audited]
	public class PersonalContact : Contact
	{
		public virtual string FirstName { get; set; }
	}
}