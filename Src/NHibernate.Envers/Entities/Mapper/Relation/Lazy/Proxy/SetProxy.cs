using System;
using System.Collections.Generic;
using Iesi.Collections.Generic;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy
{
	[Serializable]
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
			return CollectionDelegate.Clone();
		}

		public ISet<T> Union(ISet<T> a)
		{
			CheckInit();
			return CollectionDelegate.Union(a);
		}

		public ISet<T> Intersect(ISet<T> a)
		{
			CheckInit();
			return CollectionDelegate.Intersect(a);
		}

		public ISet<T> Minus(ISet<T> a)
		{
			CheckInit();
			return CollectionDelegate.Minus(a);
		}

		public ISet<T> ExclusiveOr(ISet<T> a)
		{
			CheckInit();
			return CollectionDelegate.ExclusiveOr(a);
		}

		public bool ContainsAll(ICollection<T> c)
		{
			CheckInit();
			return CollectionDelegate.ContainsAll(c);
		}

		public new bool Add(T o)
		{
			CheckInit();
			return CollectionDelegate.Add(o);
		}

		public bool AddAll(ICollection<T> c)
		{
			CheckInit();
			return CollectionDelegate.AddAll(c);
		}

		public bool RemoveAll(ICollection<T> c)
		{
			CheckInit();
			return CollectionDelegate.RemoveAll(c);
		}

		public bool RetainAll(ICollection<T> c)
		{
			CheckInit();
			return CollectionDelegate.RetainAll(c);
		}

		public bool IsEmpty
		{
			get
			{
				CheckInit();
				return CollectionDelegate.IsEmpty;
			}
		}
	}
}
