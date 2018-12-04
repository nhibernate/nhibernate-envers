using System;
using System.Collections.Generic;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;

namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy
{
	[Serializable]
	public class SetProxy<T> : CollectionProxy<T>, ISet<T>
	{
		public SetProxy(IInitializor initializor) : base(initializor)
		{
		}

		public new bool Add(T item)
		{
			return GetCollection<ISet<T>>().Add(item);
		}

		public void ExceptWith(IEnumerable<T> other)
		{
			GetCollection<ISet<T>>().ExceptWith(other);
		}

		public void IntersectWith(IEnumerable<T> other)
		{
			GetCollection<ISet<T>>().IntersectWith(other);
		}

		public bool IsProperSubsetOf(IEnumerable<T> other)
		{
			return GetCollection<ISet<T>>().IsProperSubsetOf(other);
		}

		public bool IsProperSupersetOf(IEnumerable<T> other)
		{
			return GetCollection<ISet<T>>().IsProperSupersetOf(other);
		}

		public bool IsSubsetOf(IEnumerable<T> other)
		{
			return GetCollection<ISet<T>>().IsSubsetOf(other);
		}

		public bool IsSupersetOf(IEnumerable<T> other)
		{
			return GetCollection<ISet<T>>().IsSupersetOf(other);
		}

		public bool Overlaps(IEnumerable<T> other)
		{
			return GetCollection<ISet<T>>().Overlaps(other);
		}

		public bool SetEquals(IEnumerable<T> other)
		{
			return GetCollection<ISet<T>>().SetEquals(other);
		}

		public void SymmetricExceptWith(IEnumerable<T> other)
		{
			GetCollection<ISet<T>>().SymmetricExceptWith(other);
		}

		public void UnionWith(IEnumerable<T> other)
		{
			GetCollection<ISet<T>>().UnionWith(other);
		}
	}
}