using Iesi.Collections.Generic;

namespace NHibernate.Envers.Tests.Entities.Components.Relations
{
	public class OneToManyComponent
	{
		public OneToManyComponent()
		{
			Entities = new HashedSet<StrTestEntity>();
		}

		public virtual ISet<StrTestEntity> Entities { get; set; }
		public virtual string Data { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as OneToManyComponent;
			if (other == null)
				return false;
			if (!Data.Equals(other.Data))
				return false;
			if (Entities.Count != other.Entities.Count)
				return false;
			if (!Entities.ContainsAll(other.Entities))
				return false;
			return true;
		}

		public override int GetHashCode()
		{
			return Data.GetHashCode() ^ Entities.GetHashCode();
		}
	}
}