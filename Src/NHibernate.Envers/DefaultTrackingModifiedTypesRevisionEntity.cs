using Iesi.Collections.Generic;

namespace NHibernate.Envers
{
	/// <summary>
	/// Extension of <see cref="DefaultRevisionEntity"/> that allows tracking entity types changed in each revision. This revision
	/// entity is implicitly used when <code>nhibernate.envers.track_entities_changed_in_revision</code> parameter
	/// is set to <code>true</code>.
	/// </summary>
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