using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.NotUpdatable
{
	[Audited]
	public class PropertyNotUpdatableEntity
	{
		public virtual long Id { get; set; }
		public virtual string Data { get; set; }
		public virtual string ConstantData1 { get; set; }
		public virtual string ConstantData2 { get; set; }

		public override bool Equals(object obj)
		{
			var that = obj as PropertyNotUpdatableEntity;
			if (that == null)
				return false;

			if (Data != null ? !Data.Equals(that.Data) : that.Data != null) return false;
			if (ConstantData1 != null ? !ConstantData1.Equals(that.ConstantData1) : that.ConstantData1 != null) return false;
			if (ConstantData2 != null ? !ConstantData2.Equals(that.ConstantData2) : that.ConstantData2 != null) return false;
			return Id == that.Id;
		}

		public override int GetHashCode()
		{
			var result = Id.GetHashCode();
			result = 31 * result + (Data != null ? Data.GetHashCode() : 0);
			result = 31 * result + (ConstantData1 != null ? ConstantData1.GetHashCode() : 0);
			result = 31 * result + (ConstantData2 != null ? ConstantData2.GetHashCode() : 0);
			return result;
		}
	}
}