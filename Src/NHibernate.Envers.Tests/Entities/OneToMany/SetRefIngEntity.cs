namespace NHibernate.Envers.Tests.Entities.OneToMany
{
    public class SetRefIngEntity
    {
        public virtual int Id { get; set; }

        [Audited]
        public virtual string Data { get; set; }

        [Audited]
        public virtual SetRefEdEntity Reference { get; set; }

        public override bool Equals(object obj)
        {
            var casted = obj as SetRefEdEntity;
            if (casted == null)
                return false;
            return (Id == casted.Id && Data == casted.Data);
        }

        public override int GetHashCode()
        {
            return Id ^ Data.GetHashCode();
        }
    }
}