namespace NHibernate.Envers.Tests.Integration.Inheritance.Mixed
{
	public class ActivityId
	{
		public virtual int Id1 { get; set; }
		public virtual int Id2 { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as ActivityId;
			if (casted == null)
				return false;
			return Id1 == casted.Id1 && Id2 == casted.Id2;
		}

		public override int GetHashCode()
		{
			return Id1 ^ Id2;
		}
	}
}