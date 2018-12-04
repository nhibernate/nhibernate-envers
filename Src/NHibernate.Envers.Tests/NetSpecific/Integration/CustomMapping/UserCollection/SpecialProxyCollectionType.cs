using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.CustomMapping.UserCollection
{
	public class SpecialProxyCollectionType : ListProxy<Number>, ISpecialCollection
	{
		public SpecialProxyCollectionType(IInitializor initializor) : base(initializor)
		{
		}

		public int ItemsOverLimit()
		{
			return GetCollection<ISpecialCollection>().ItemsOverLimit();
		}

		public int Limit => GetCollection<ISpecialCollection>().Limit;
	}
}