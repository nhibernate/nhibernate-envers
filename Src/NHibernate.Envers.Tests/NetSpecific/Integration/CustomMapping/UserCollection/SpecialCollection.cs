using System.Collections.ObjectModel;
using System.Linq;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.CustomMapping.UserCollection
{
	public class SpecialCollection : Collection<Number>, ISpecialCollection
	{
		public SpecialCollection(int limit)
		{
			Limit = limit;
		}

		public int Limit { get; private set; }

		public int ItemsOverLimit()
		{
			return this.Count(item => item.Value > Limit);
		}
	}
}