namespace NHibernate.Envers.Tools
{
    public class Triple<T1, T2, T3> 
	{
        public T1 First { get; private set; }
        public T2 Second { get; private set; }
        public T3 Third { get; private set; }

        public Triple(T1 First, T2 Second, T3 Third) 
		{
            this.First = First;
            this.Second = Second;
            this.Third = Third;
        }

        public override bool Equals(object o) 
		{
            if (this == o) return true;
            if (!(o is Triple<T1, T2, T3>)) return false;

            var triple = (Triple<T1, T2, T3>) o;

            if (First != null ? !First.Equals(triple.First) : triple.First != null) return false;
            if (Second != null ? !Second.Equals(triple.Second) : triple.Second != null) return false;
            if (Third != null ? !Third.Equals(triple.Third) : triple.Third != null) return false;

            return true;
        }

        public override int GetHashCode() 
		{
        	int result = (First != null ? First.GetHashCode() : 0);
            result = 31 * result + (Second != null ? Second.GetHashCode() : 0);
            result = 31 * result + (Third != null ? Third.GetHashCode() : 0);
            return result;
        }

        public static Triple<T1, T2, T3> Make(T1 First, T2 Second, T3 Third)
        {
            return new Triple<T1, T2, T3>(First, Second, Third);
        }
    }
}
