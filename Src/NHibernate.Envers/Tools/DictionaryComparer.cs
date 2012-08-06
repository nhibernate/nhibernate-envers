using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Envers.Tools
{
	public class DictionaryComparer<K, V> : IEqualityComparer<IDictionary<K, V>>
	{
		public bool Equals(IDictionary<K, V> x, IDictionary<K, V> y)
		{
			if (x.Count != y.Count)
				return false;
			foreach (var xKeyValue in x)
			{
				V yValue;
				if (y.TryGetValue(xKeyValue.Key, out yValue))
				{
					if (!xKeyValue.Value.Equals(yValue))
						return false;
				}
				else
				{
					return false;
				}
			}
			return true;
		}

		public int GetHashCode(IDictionary<K, V> obj)
		{
			return obj.Aggregate(123, (current, keyValue) => current + (keyValue.GetHashCode()*31));
		}
	}
}