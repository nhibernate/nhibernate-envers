namespace NHibernate.Envers.Tests.Integration.Ids.EmbeddedId
{
	public class ItemId
	{
		public virtual string Model { get; set; }
		public virtual int Version { get; set; }
		public virtual Producer Producer { get; set; }

		public override bool Equals(object obj)
		{
			var itemId = obj as ItemId;
			if (itemId == null)
				return false;
			if (Model != null ? !Model.Equals(itemId.Model) : itemId.Model != null) 
				return false;
			if (Producer != null ? !Producer.Equals(itemId.Producer) : itemId.Producer != null) 
				return false;
			if (Version != itemId.Version)
				return false;
			return true;
		}

		public override int GetHashCode()
		{
			var res = Model != null ? Model.GetHashCode() : 0;
			res = 31*res + Version.GetHashCode();
			res = 31*res + (Producer != null ? Producer.GetHashCode() : 0);
			return res;
		}
	}
}