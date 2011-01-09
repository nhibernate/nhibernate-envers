namespace NHibernate.Envers.Tests.Entities.Components
{
	public class Component2
	{
		public virtual string Str5 { get; set; }
		public virtual string Str6 { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as Component2;
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}
			return Str5.Equals(other.Str5) && Str6.Equals(other.Str6);
		}

		public override int GetHashCode()
		{
			return Str5.GetHashCode() ^ Str6.GetHashCode();
		}
	}
}