using System.Collections.Generic;
using NHibernate.Envers.Tests.Entities;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.SortedSetAndMap
{
	public class StrTestEntityComparer : IComparer<StrTestEntity>
	{
		public int Compare(StrTestEntity x, StrTestEntity y)
		{
			return y.Str.CompareTo(x.Str);
		}
	}
}