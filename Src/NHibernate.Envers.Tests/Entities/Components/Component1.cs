namespace NHibernate.Envers.Tests.Entities.Components
{
	public class Component1
	{
		public virtual string Str1 { get; set; }
		public virtual string Str2 { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as Component1;
			if (other == null)
			{
				return false;
			}
			if (Str1 != null ? !Str1.Equals(other.Str1) : other.Str1 != null) return false;
			if (Str2 != null ? !Str2.Equals(other.Str2) : other.Str2 != null) return false;

			return true;
		}

		public override int GetHashCode()
		{
			var result = (Str1 != null ? Str1.GetHashCode() : 0);
			return 31 * result + (Str2 != null ? Str2.GetHashCode() : 0);
		}
	}
}