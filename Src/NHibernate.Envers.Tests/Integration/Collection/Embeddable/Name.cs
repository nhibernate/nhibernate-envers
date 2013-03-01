using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Collection.Embeddable
{
	[Audited]
	public class Name
	{
		public virtual string FirstName { get; set; }
		public virtual string LastName { get; set; }

		public override bool Equals(object obj)
		{
			var that = obj as Name;
			if (that == null)
				return false;
			if (FirstName != null ? !FirstName.Equals(that.FirstName) : that.FirstName != null) return false;
			if (LastName != null ? !LastName.Equals(that.LastName) : that.LastName != null) return false;
			return true;
		}

		public override int GetHashCode()
		{
			var result = FirstName != null ? FirstName.GetHashCode() : 0;
			result = 31 * result + (LastName != null ? LastName.GetHashCode() : 0);
			return result;
		}
	}
}