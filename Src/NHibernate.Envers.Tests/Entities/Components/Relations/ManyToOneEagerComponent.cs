using NHibernate.Envers.Tests.Entities.Collection;

namespace NHibernate.Envers.Tests.Entities.Components.Relations
{
	public class ManyToOneEagerComponent
	{
		public virtual StrTestNoProxyEntity Entity { get; set; }
		public virtual string Data { get; set; }

		public override bool Equals(object obj)
		{
			var that = obj as ManyToOneEagerComponent;
			if (that == null)
				return false;
			if (Data != null ? !Data.Equals(that.Data) : that.Data != null) return false;
			if (Entity != null ? !Entity.Equals(that.Entity) : that.Entity != null) return false;
			return true;
		}

		public override int GetHashCode()
		{
			var result = Entity != null ? Entity.GetHashCode() : 0;
			result = 31 * result + (Data != null ? Data.GetHashCode() : 0);
			return result;
		}
	}
}