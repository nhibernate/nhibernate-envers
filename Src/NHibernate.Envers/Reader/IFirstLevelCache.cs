namespace NHibernate.Envers.Reader
{
    /**
     * First level cache for versioned entities, versions reader-scoped. Each entity is uniquely identified by a
     * revision number and entity id.
     * @author Simon Duduica, created interface that separes the first level cache implementation.
     * Default implementation will be FirstLevelCache to be ported in the phase 2
     */
    public interface IFirstLevelCache 
	{
        object this[string entityName, long revision, object id] { get; }
        void Add(string entityName, long revision, object id, object entity);
        bool Contains(string entityName, long revision, object id);
    }
}
