namespace NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor
{
    public interface IInitializor<T> 
    {
        T Initialize();
    }
}
