using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Tests.Entities.Components;

namespace NHibernate.Envers.Tests.Integration.ModifiedFlags.Entities
{
	[Audited(WithModifiedFlag = false)]
	public class PartialModifiedFlagsEntity
	{
		public PartialModifiedFlagsEntity()
		{
			StringSet = new HashSet<string>();
			EntitiesSet = new HashSet<StrTestEntity>();
			StringMap = new Dictionary<string, string>();
			EntitiesMap = new Dictionary<string, StrTestEntity>();
		}

		public virtual int Id { get; set; }
		[Audited(WithModifiedFlag = true)]
		public virtual string Data { get; set; }
		[Audited(WithModifiedFlag = true)]
		public virtual Component1 Comp1 { get; set; }
		[Audited(WithModifiedFlag = false)]
		public virtual Component2 Comp2 { get; set; }
		[Audited(WithModifiedFlag = true)]
		public virtual WithModifiedFlagReferencingEntity Referencing { get; set; }
		[Audited(WithModifiedFlag = false)]
		public virtual WithModifiedFlagReferencingEntity Referencing2 { get; set; }
		[Audited(WithModifiedFlag = true)]
		public virtual ISet<string> StringSet { get; set; }
		[Audited(WithModifiedFlag = true)]
		[AuditJoinTable(TableName = "Entitiesset_AUD")]
		public virtual ISet<StrTestEntity> EntitiesSet { get; set; }
		[Audited(WithModifiedFlag = true)]
		public virtual IDictionary<string, string> StringMap { get; set; }
		[Audited(WithModifiedFlag = true)]
		[AuditJoinTable(TableName = "Entitiesmap_AUD")]
		public virtual IDictionary<string, StrTestEntity> EntitiesMap { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as PartialModifiedFlagsEntity;
			if (casted == null)
				return false;
			if (Data != null ? !Data.Equals(casted.Data) : casted.Data != null) 
				return false;
			if (Id != casted.Id)
				return false;

			return true;
		}

		public override int GetHashCode()
		{
			var result = Id;
			result = 31 * result + (Data != null ? Data.GetHashCode() : 0);
			return result;
		}
	}
}