namespace NHibernate.Envers.Tests.Integration.NotInsertable
{
    [Audited]
    public class NotInsertableTestEntity
    {
        public virtual int Id { get; set; }
        public virtual string Data { get; set; }
        public virtual string DataCopy { get; set; }

        public NotInsertableTestEntity()
        {
            Data = string.Empty;
            DataCopy = string.Empty;
        }

        public override bool Equals(object obj)
        {
            var casted = obj as NotInsertableTestEntity;
            if (casted == null)
                return false;
            return (Id == casted.Id && Data.Equals(casted.Data) && DataCopy.Equals(casted.DataCopy));
        }

        public override int GetHashCode()
        {
            return Id ^ Data.GetHashCode() ^ DataCopy.GetHashCode();
        }
    }
}