using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Properties
{
	public class PropertiesTestEntity
	{
		public virtual int Id { get; set; }
		[Audited]
		public virtual string Str { get; set; }

		public override bool Equals(object obj)
		{
			var that = obj as PropertiesTestEntity;
			if (that == null)
				return false;
			if (Str != null ? !Str.Equals(that.Str) : that.Str != null)
				return false;
			return Id == that.Id;
		}

		public override int GetHashCode()
		{
			var strHash = Str != null ? Str.GetHashCode() : 0;
			return Id ^ strHash;
		}
	}
}