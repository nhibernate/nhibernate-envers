using System;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Initializor;
using NHibernate.Envers.Entities.Mapper.Relation.Lazy.Proxy;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.CustomMapping.UserCollection
{
	public class SpecialProxyCollectionType : ListProxy<Number>, ISpecialCollection
	{
		private Lazy<ISpecialCollection> _delegate;

		public SpecialProxyCollectionType(IInitializor initializor) : base(initializor)
		{
			_delegate = new Lazy<ISpecialCollection>(() => (ISpecialCollection) initializor.Initialize());
		}

		public int ItemsOverLimit()
		{
			return _delegate.Value.ItemsOverLimit();
		}

		public int Limit => _delegate.Value.Limit;
	}
}