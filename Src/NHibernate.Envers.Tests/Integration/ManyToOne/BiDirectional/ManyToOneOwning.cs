using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.ManyToOne.BiDirectional
{
	[Audited]
	public class ManyToOneOwning
	{
		public virtual long Id { get; set; }
		public virtual string Data { get; set; }
		public virtual OneToManyOwned References { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as ManyToOneOwning;
			if (casted == null)
				return false;
			if (Data != null ? !Data.Equals(casted.Data) : casted.Data != null) return false;
			return Id == casted.Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Data.GetHashCode();
		}
	}
}