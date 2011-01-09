namespace NHibernate.Envers.Tests.Integration.NotInsertable.ManyToOne
{
    [Audited]
    public class NotInsertableEntityType
    {
        public virtual int TypeId { get; set; }
        public virtual string Type { get; set; }

        public override bool Equals(object obj)
        {
            var casted = obj as NotInsertableEntityType;
            if (casted == null)
                return false;
            return (TypeId == casted.TypeId && Type.Equals(casted.Type));
        }

        public override int GetHashCode()
        {
            return TypeId ^ Type.GetHashCode();
        }
    }
}