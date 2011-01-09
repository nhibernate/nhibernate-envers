namespace NHibernate.Envers.Tests.Entities.Components.Relations
{
	public class ManyToOneComponent
	{
		public StrTestEntity Entity { get; set; }
		public string Data { get; set; }

		public override bool Equals(object obj)
		{
			var that = obj as ManyToOneComponent;
			if (that == null)
				return false;
			return Entity.Equals(that.Entity) && Data.Equals(that.Data);
		}

		public override int GetHashCode()
		{
			return Entity.GetHashCode() ^ Data.GetHashCode();
		}
	}
}