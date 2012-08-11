using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Ids
{
	[Audited]
	public class MultipleIdEntity
	{
		public virtual int Id1 { get; set; } 
		public virtual int Id2 { get; set; }

		public virtual int Value { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as MultipleIdEntity;
			if (other == null)
				return false;
			return Id1 == other.Id1 && Id2 == other.Id2;
		}

		public override int GetHashCode()
		{
			return Id1.GetHashCode() ^ Id2.GetHashCode();
		}
	}
}