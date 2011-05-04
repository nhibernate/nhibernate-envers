using System;

namespace NHibernate.Envers.Tools
{
	public class Triple<T1, T2, T3> : IEquatable<Triple<T1, T2, T3>>
	{
		private readonly int hashCode;

		public Triple(T1 first, T2 second, T3 third)
		{
			First = first;
			Second = second;
			Third = third;
			unchecked
			{
				hashCode = ReferenceEquals(First, null) ? 17 : First.GetHashCode();
				hashCode = (hashCode*397) ^ (ReferenceEquals(Second, null) ? 19 : Second.GetHashCode());
				hashCode = (hashCode*397) ^ (ReferenceEquals(Third, null) ? 23 : Third.GetHashCode());
			}
		}

		public T1 First { get; private set; }
		public T2 Second { get; private set; }
		public T3 Third { get; private set; }

		public override bool Equals(object obj)
		{
			var castedObj = obj as Triple<T1, T2, T3>;
			return castedObj != null && Equals(castedObj);
		}

		public bool Equals(Triple<T1, T2, T3> other)
		{
			if (other == null)
			{
				return false;
			}
			if (this == other)
			{
				return true;
			}
			return other.First.Equals(First) && other.Second.Equals(Second) && other.Third.Equals(Third);
		}

		public override int GetHashCode()
		{
			return hashCode;
		}
	}
}