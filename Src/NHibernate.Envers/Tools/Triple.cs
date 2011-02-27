namespace NHibernate.Envers.Tools
{
	public class Triple<T1, T2, T3>
	{
		private readonly int hashCode;

		internal Triple(T1 first, T2 second, T3 third)
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
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != typeof (Triple<T1, T2, T3>))
			{
				return false;
			}
			return Equals((Triple<T1, T2, T3>) obj);
		}

		public static Triple<T1, T2, T3> Make(T1 first, T2 second, T3 third)
		{
			return new Triple<T1, T2, T3>(first, second, third);
		}

		public bool Equals(Triple<T1, T2, T3> other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.First, First) && Equals(other.Second, Second) && Equals(other.Third, Third);
		}

		public override int GetHashCode()
		{
			return hashCode;
		}
	}
}