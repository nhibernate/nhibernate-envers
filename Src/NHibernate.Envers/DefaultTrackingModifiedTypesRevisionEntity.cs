using Iesi.Collections.Generic;

namespace NHibernate.Envers
{
	public class DefaultTrackingModifiedTypesRevisionEntity : DefaultRevisionEntity
	{
		public DefaultTrackingModifiedTypesRevisionEntity()
		{
			ModifiedEntityTypes = new HashedSet<string>();
		}

		public virtual ISet<string> ModifiedEntityTypes { get; protected set; }

		public override bool Equals(object obj)
		{
			if (this == obj)
				return true;
			var casted = obj as DefaultTrackingModifiedTypesRevisionEntity;
			if (casted == null)
				return false;
			if (!base.Equals(obj))
				return false;
			return ModifiedEntityTypes.Equals(casted.ModifiedEntityTypes);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ ModifiedEntityTypes.GetHashCode();
		}
	}
}