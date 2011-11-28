using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Merge
{
	[Audited]
	public class GivenIdStrEntity
	{
		public virtual int Id { get; set; }
		public virtual string Data { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as GivenIdStrEntity;
			if (casted == null)
				return false;
			if (Data != null ? !Data.Equals(casted.Data) : casted.Data != null)
				return false;
			return Id == casted.Id;
		}

		public override int GetHashCode()
		{
			var strHash = (Data != null ? Data.GetHashCode() : 0);
			return strHash ^ Id;
		}
	}
}