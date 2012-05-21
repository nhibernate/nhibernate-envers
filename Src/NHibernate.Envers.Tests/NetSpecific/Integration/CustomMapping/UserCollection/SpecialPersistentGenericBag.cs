using System.Collections.Generic;
using NHibernate.Collection.Generic;
using NHibernate.Engine;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.CustomMapping.UserCollection
{
	public class SpecialPersistentGenericBag : PersistentGenericBag<Number>, ISpecialCollection
	{
		public SpecialPersistentGenericBag(ISessionImplementor session, ICollection<Number> list):base(session, list)
		{
		}

		public SpecialPersistentGenericBag(ISessionImplementor session) : base(session)
		{
		}

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

		public int Limit
		{
			get { return ((ISpecialCollection)InternalBag).Limit; }
		}
	}
}