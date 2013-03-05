using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.Ids
{
	[Audited]
	public class UnusualIdNamingEntity
	{
		public virtual string UniqueField { get; set; }
		public virtual string VariousData { get; set; }

		public override bool Equals(object obj)
		{
			var that = obj as UnusualIdNamingEntity;
			if (that == null)
				return false;

			if (UniqueField != null ? !UniqueField.Equals(that.UniqueField) : that.UniqueField != null) return false;
			if (VariousData != null ? !VariousData.Equals(that.VariousData) : that.VariousData != null) return false;
			return true;
		}

		public override int GetHashCode()
		{
			var result = UniqueField != null ? UniqueField.GetHashCode() : 0;
			result = 31 * result + (VariousData != null ? VariousData.GetHashCode() : 0);
			return result;
		}
	}
}