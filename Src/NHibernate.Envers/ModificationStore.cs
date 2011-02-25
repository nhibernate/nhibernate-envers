namespace NHibernate.Envers
{
    public enum ModificationStore
    {
        None, // C# does not accept null values passed for an enum value so we use this.
        Full
    }
}
