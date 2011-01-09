using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;

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
		public virtual IDictionary<int, StrTestEntity> IdMap { get; set; }
	}
}