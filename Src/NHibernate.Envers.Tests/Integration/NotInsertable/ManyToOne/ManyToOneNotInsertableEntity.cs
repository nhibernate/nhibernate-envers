namespace NHibernate.Envers.Tests.Integration.NotInsertable.ManyToOne
{
    [Audited]
    public class ManyToOneNotInsertableEntity
    {
        public virtual int Id { get; set; }
        public virtual int Number { get; set; }
        public virtual NotInsertableEntityType Type { get; set; }
    }
}