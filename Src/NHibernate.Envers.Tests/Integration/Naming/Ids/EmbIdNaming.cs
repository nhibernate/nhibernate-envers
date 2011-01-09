namespace NHibernate.Envers.Tests.Integration.Naming.Ids
{
    public class EmbIdNaming
    {
        public int X { get; set; }
        public int Y { get; set; }

        public override bool Equals(object obj)
        {
            var casted = obj as EmbIdNaming;
            if (casted == null)
                return false;
            return X == casted.X && Y == casted.Y;
        }

        public override int GetHashCode()
        {
            return X ^ Y;
        }
    }
}