using System.Collections.Generic;
using Iesi.Collections.Generic;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy
{
	public class SetProxy<T>: CollectionProxy<T, ISet<T>>, ISet<T> 
	{
		public SetProxy() 
		{
		}

		public SetProxy(IInitializor<ISet<T>> initializor)
						:base(initializor) 
		{
		}

		public object Clone()
		{
			CheckInit();
			return delegat.Clone();
		}

		public ISet<T> Union(ISet<T> a)
		{
			CheckInit();
			return delegat.Union(a);
		}

		public ISet<T> Intersect(ISet<T> a)
		{
			CheckInit();
			return delegat.Intersect(a);
		}

		public ISet<T> Minus(ISet<T> a)
		{
			CheckInit();
			return delegat.Minus(a);
		}

		public ISet<T> ExclusiveOr(ISet<T> a)
		{
			CheckInit();
			return delegat.ExclusiveOr(a);
		}

		public bool ContainsAll(ICollection<T> c)
		{
			CheckInit();
			return delegat.ContainsAll(c);
		}

		public new bool Add(T o)
		{
			CheckInit();
			return delegat.Add(o);
		}

		public bool AddAll(ICollection<T> c)
		{
			CheckInit();
			return delegat.AddAll(c);
		}

		public bool RemoveAll(ICollection<T> c)
		{
			CheckInit();
			return delegat.RemoveAll(c);
		}

		public bool RetainAll(ICollection<T> c)
		{
			CheckInit();
			return delegat.RetainAll(c);
		}

		public bool IsEmpty
		{
			get
			{
				CheckInit();
				return delegat.IsEmpty;
			}
		}
	}
}
