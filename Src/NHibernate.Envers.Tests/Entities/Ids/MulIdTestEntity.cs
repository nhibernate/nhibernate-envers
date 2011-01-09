namespace NHibernate.Envers.Tests.Entities.Ids
{
	public class MulIdTestEntity
	{
		public virtual int Id1 { get; set; }
		public virtual int Id2 { get; set; }
		[Audited]
		public virtual string Str1 { get; set; }


		public override bool Equals(object obj)
		{
			var casted = obj as MulIdTestEntity;
			if (casted == null)
				return false;
			return (Id1 == casted.Id1 && Id2 == casted.Id2 && Str1.Equals(casted.Str1));
		}

		public override int GetHashCode()
		{
			return Id1.GetHashCode() ^ Id2.GetHashCode() ^ Str1.GetHashCode();
		}
	}
}