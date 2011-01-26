namespace NHibernate.Envers.Tests.Integration.SuperClass
{
	public class SuperclassOfEntity
	{
		[Audited]
		public virtual string Str { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as SuperclassOfEntity;
			return other != null && Str.Equals(other.Str);
		}

		public override int GetHashCode()
		{
			return Str.GetHashCode();
		}
	}
}