using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.JoinAuditTable
{
	[Audited]
	[JoinAuditTable(JoinAuditTableName = "HeightTableAuditing", JoinTableName = "HeightTable")]
	[JoinAuditTable(JoinAuditTableName = "WeightTableAuditing", JoinTableName = "WeightTable")]
	public class Animal
	{
		public virtual int Id { get; set; }
		public virtual int Height { get; set; }
		public virtual int Weight { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as Animal;
			if (other == null)
				return false;
			return Id == other.Id && Height == other.Height && Weight == other.Weight;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Height.GetHashCode() ^ Weight.GetHashCode();
		}
	}
}