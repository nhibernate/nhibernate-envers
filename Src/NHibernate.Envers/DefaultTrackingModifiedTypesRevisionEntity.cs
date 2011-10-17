using Iesi.Collections.Generic;

namespace NHibernate.Envers
{
	public class DefaultTrackingModifiedTypesRevisionEntity : DefaultRevisionEntity
	{
		public DefaultTrackingModifiedTypesRevisionEntity()
		{
			ModifiedEntityNames = new HashedSet<string>();
		}

		public virtual ISet<string> ModifiedEntityNames { get; protected set; }

		public override bool Equals(object obj)
		{
			if (this == obj)
				return true;
			var casted = obj as DefaultTrackingModifiedTypesRevisionEntity;
			if (casted == null)
				return false;
			if (!base.Equals(obj))
				return false;
			return ModifiedEntityNames.Equals(casted.ModifiedEntityNames);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ ModifiedEntityNames.GetHashCode();
		}
	}
}