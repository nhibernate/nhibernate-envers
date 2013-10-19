using System.Collections.Generic;

namespace NHibernate.Envers
{
	/// <summary>
	/// Extension of <see cref="DefaultRevisionEntity"/> that allows tracking entity types changed in each revision. This revision
	/// entity is implicitly used when <code>nhibernate.envers.track_entities_changed_in_revision</code> parameter
	/// is set to <code>true</code>.
	/// </summary>
	public class DefaultTrackingModifiedEntitiesRevisionEntity : DefaultRevisionEntity
	{
		public DefaultTrackingModifiedEntitiesRevisionEntity()
		{
			ModifiedEntityNames = new HashSet<string>();
		}

		public virtual ISet<string> ModifiedEntityNames { get; protected set; }

		public override bool Equals(object obj)
		{
			if (this == obj)
				return true;
			var casted = obj as DefaultTrackingModifiedEntitiesRevisionEntity;
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