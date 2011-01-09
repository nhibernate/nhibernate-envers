namespace NHibernate.Envers.Tests.Integration.Inheritance.Entities
{
	public class PersonalContact : Contact
	{
		public virtual string FirstName { get; set; }
	}
}