namespace NHibernate.Envers.Tests.Integration.Data
{
	public class SerializableTestEntity
	{
		public virtual int Id { get; set; }
		[Audited]
		public virtual SerObj Obj { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as SerializableTestEntity;
			if (other == null)
				return false;
			return other.Id == Id && other.Obj.Equals(Obj);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Obj.GetHashCode();
		}
	}
}