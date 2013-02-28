using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.Collection
{
	public class StrTestNoProxyEntity
	{
		public virtual int Id { get; set; }
		[Audited]
		public virtual string Str { get; set; }

		public override bool Equals(object obj)
		{
			var that = obj as StrTestNoProxyEntity;
			if (that == null)
				return false;
			if (Id != that.Id) return false;
			if (Str != null ? !Str.Equals(that.Str) : that.Str != null) return false;
			return true;
		}

		public override int GetHashCode()
		{
			var result = Id.GetHashCode();
			result = 31 * result + (Str != null ? Str.GetHashCode() : 0);
			return result;
		}
	}
}