using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Ids.EmbeddedId
{
	[Audited]
	public class Item
	{
		public virtual ItemId Id { get; set; }
		public virtual double Price { get; set; }

		public override bool Equals(object obj)
		{
			var item = obj as Item;
			if (item == null)
				return false;
			if (Id != null ? !Id.Equals(item.Id) : item.Id != null)
				return false;
			if (Price != item.Price)
				return false;
			return true;
		}

		public override int GetHashCode()
		{
			var result = Id != null ? Id.GetHashCode() : 0;
			result = 31*result + Price.GetHashCode();
			return result;
		}
	}
}