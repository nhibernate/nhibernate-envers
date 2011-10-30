using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Entities
{
	[Audited]
	public class Person : RightsSubject
	{
		public Person()
		{
			Name = string.Empty;
		}

		public virtual string Name { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as Person;
			if (casted == null)
				return false;
			if (!base.Equals(obj))
				return false;
			return Name.Equals(casted.Name);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ Name.GetHashCode();
		}
	}
}