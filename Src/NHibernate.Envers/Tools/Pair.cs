namespace NHibernate.Envers.Tools
{
	public class Pair<T1, T2> : IPair
	{
		public T1 First{ get; private set;}
		public T2 Second { get; private set; }

		object IPair.First
		{
			get { return First; }
		}

		object IPair.Second
		{
			get { return Second; }
		}

		private Pair(T1 obj1, T2 obj2)
		{
			First = obj1;
			Second = obj2;
		}

		public override bool Equals(object o) 
		{
			if (this == o) return true;

			var pair = (Pair<T1, T2>) o;

			if (First != null ? !First.Equals(pair.First) : pair.First != null) return false;
			if (Second != null ? !Second.Equals(pair.Second) : pair.Second != null) return false;

			return true;
		}

		public override int GetHashCode() 
		{
			var result = (First != null ? First.GetHashCode() : 0);
			result = 31 * result + (Second != null ? Second.GetHashCode() : 0);
			return result;
		}

		public static Pair<T1, T2> Make(T1 obj1, T2 obj2)
		{
			return new Pair<T1, T2>(obj1, obj2);
		}
	}
}
