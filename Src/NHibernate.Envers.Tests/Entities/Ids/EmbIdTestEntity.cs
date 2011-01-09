namespace NHibernate.Envers.Tests.Entities.Ids
{
	public class EmbIdTestEntity
	{
		public virtual EmbId Id { get; set; }
		[Audited]
		public virtual string Str1 { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as EmbIdTestEntity;
			if (casted == null)
				return false;
			return (Id.Equals(casted.Id) && Str1.Equals(casted.Str1));
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Str1.GetHashCode();
		}
	}
}