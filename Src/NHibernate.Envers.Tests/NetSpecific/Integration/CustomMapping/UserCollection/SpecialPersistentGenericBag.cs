using System.Collections.Generic;
using NHibernate.Collection.Generic;
using NHibernate.Engine;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.CustomMapping.UserCollection
{
	public class SpecialPersistentGenericBag : PersistentGenericBag<Number>, ISpecialCollection
	{
		public SpecialPersistentGenericBag(ISessionImplementor session, ICollection<Number> list, int limit):base(session, list)
		{
			Limit = limit;
		}

		public SpecialPersistentGenericBag(ISessionImplementor session, int limit)
			: base(session)
		{
			Limit = limit;
		}

		public int Limit { get; private set; }

		public int ItemsOverLimit()
		{
			var ret=0;
			foreach (Number item in this)
			{
				if (item.Value > Limit)
					ret++;
			}
			return ret;
		}		
	}
}