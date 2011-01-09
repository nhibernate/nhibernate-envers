namespace NHibernate.Envers.Tests.Integration.HashCode
{
	[Audited]
	public class WikiImage
	{
		public virtual int Id { get; set; }
		public virtual string Name { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as WikiImage;
			if (other == null)
				return false;
			if (Name != null ? !Name.Equals(other.Name) : other.Name != null) return false;

			return true;
		}

		public override int GetHashCode()
		{
			return Name != null ? Name.GetHashCode() : 0;
		}
	}
}