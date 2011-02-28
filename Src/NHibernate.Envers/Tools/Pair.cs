namespace NHibernate.Envers.Tools
{
	public class Pair<T1, T2> : IPair
	{
		private readonly int hashCode;

		internal Pair(T1 first, T2 second)
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

		#region IPair Members

		object IPair.First
		{
			get { return First; }
		}

		object IPair.Second
		{
			get { return Second; }
		}

		#endregion

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
			if (obj.GetType() != typeof (Pair<T1, T2>))
			{
				return false;
			}
			return Equals((Pair<T1, T2>) obj);
		}

		public static Pair<T1, T2> Make(T1 first, T2 second)
		{
			return new Pair<T1, T2>(first, second);
		}

		public bool Equals(Pair<T1, T2> other)
		{
			if (ReferenceEquals(null, other))
			{
				return false;
			}
			if (ReferenceEquals(this, other))
			{
				return true;
			}
			return Equals(other.First, First) && Equals(other.Second, Second);
		}

		public override int GetHashCode()
		{
			return hashCode;
		}
	}
}