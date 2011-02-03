using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;
using NHibernate.Envers.Compatibility.Attributes;

namespace NHibernate.Envers.Tests.Integration.Collection.MapKey
{
	public class IdMapKeyEntity
	{
		public IdMapKeyEntity()
		{
			IdMap = new Dictionary<int, StrTestEntity>();
		}

		public virtual int Id { get; set; }
		[Audited]
        [MapKey]
		public virtual IDictionary<int, StrTestEntity> IdMap { get; set; }
	}
}