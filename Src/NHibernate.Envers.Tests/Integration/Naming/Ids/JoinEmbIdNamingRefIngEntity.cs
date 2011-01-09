namespace NHibernate.Envers.Tests.Integration.Naming.Ids
{
    public class JoinEmbIdNamingRefIngEntity
    {
        public virtual EmbIdNaming Id { get; set; }

        [Audited]
        public virtual string Data { get; set; }

        [Audited]
        public virtual JoinEmbIdNamingRefEdEntity Reference { get; set; }

        public override bool Equals(object obj)
        {
            var casted = obj as JoinEmbIdNamingRefIngEntity;
            if (casted == null)
                return false;
            return Id.Equals(casted.Id) && Data.Equals(casted.Data);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode() ^ Data.GetHashCode();
        }
    }
}