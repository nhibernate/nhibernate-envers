using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Ids.EmbeddedId
{
	[Audited]
	public class PurchaseOrder
	{
		public virtual int Id { get; set; }
		public virtual Item Item { get; set; }
		public virtual string Comment { get; set; }

		public override bool Equals(object obj)
		{
			var that = obj as PurchaseOrder;
			if (that == null)
				return false;
			if (Comment != null ? !Comment.Equals(that.Comment) : that.Comment != null) 
				return false;
			if (Item != null ? !Item.Equals(that.Item) : that.Item != null) 
				return false;
			if (Id != that.Id)
				return false;
			return true;
		}

		public override int GetHashCode()
		{
			var result = Id;
			result = 31 * result + (Item != null ? Item.GetHashCode() : 0);
			result = 31 * result + (Comment != null ? Comment.GetHashCode() : 0);
			return result;
		}
	}
}