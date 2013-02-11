using System;

namespace NHibernate.Envers.Tools
{
	public class Pair<T1, T2> : IEquatable<Pair<T1, T2>>
	{
		private readonly int hashCode;

		public Pair(T1 first, T2 second)
		{
			First = first;
			Second = second;
			unchecked
			{
				hashCode = ReferenceEquals(First, null) ? 17 : First.GetHashCode();
				hashCode = (hashCode*397) ^ (ReferenceEquals(Second, null) ? 19 : Second.GetHashCode());
			}
		}

		public T1 First { get; private set; }
		public T2 Second { get; private set; }
		
		public override bool Equals(object obj)
		{
			var castedPair = obj as Pair<T1, T2>;
			return castedPair != null && Equals(castedPair);
		}

		public bool Equals(Pair<T1, T2> other)
		{
			if (other==null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			return other.First.Equals(First) && other.Second.Equals(Second);
		}

		public override int GetHashCode()
		{
			return hashCode;
		}
	}
}