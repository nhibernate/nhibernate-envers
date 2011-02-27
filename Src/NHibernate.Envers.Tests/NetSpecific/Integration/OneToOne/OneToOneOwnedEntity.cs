using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.OneToOne
{
	[Audited]
	public class OneToOneOwnedEntity
	{
		public virtual int Id { get; set; }
		public virtual OneToOneOwningEntity Owning { get; set; }
		public virtual string Data { get; set; }


		public override bool Equals(object obj)
		{
			var casted = obj as OneToOneOwnedEntity;
			if (casted == null)
				return false;
			return (Id == casted.Id && Data.Equals(casted.Data));
		}

		public override int GetHashCode()
		{
			return Id ^Data.GetHashCode();
		}
	}
}