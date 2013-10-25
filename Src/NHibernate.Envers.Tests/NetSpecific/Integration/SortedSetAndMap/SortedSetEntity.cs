using System;
using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Tests.Entities;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.SortedSetAndMap
{
	[Audited]
	public class SortedSetEntity
	{
		public SortedSetEntity()
		{
			SortedSet = new SortedSet<StrTestEntity>(new StrTestEntityComparer());
			SortedMap = new SortedDictionary<StrTestEntity, string>(new StrTestEntityComparer());
		}

		public virtual Guid Id { get; set; }
		[AuditJoinTable(TableName = "SSE_SS_AUD")]
		public virtual ISet<StrTestEntity> SortedSet { get; set; }
		public virtual IDictionary<StrTestEntity, string> SortedMap { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as SortedSetEntity;
			if (other == null)
				return false;
			return Id == other.Id;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

	}
}