using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.SameIds
{
	public class SameIdTestEntity2
	{
		public virtual int Id { get; set; }
		[Audited]
		public virtual string Str1 { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as SameIdTestEntity2;
			if (other == null)
				return false;
			return Id == other.Id && Str1.Equals(other.Str1);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Str1.GetHashCode();
		}
	}
}