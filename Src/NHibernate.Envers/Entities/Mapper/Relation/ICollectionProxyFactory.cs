using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	public interface ICollectionProxyFactory
	{
		object Create(System.Type collectionInterface, IInitializor collectionInitializor);
	}
}