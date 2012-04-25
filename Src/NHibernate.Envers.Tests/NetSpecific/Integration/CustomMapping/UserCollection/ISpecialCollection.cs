using System.Collections.Generic;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.CustomMapping.UserCollection
{
	public interface ISpecialCollection : IList<Number>
	{
		int ItemsOverLimit();
		int Limit { get; }
	}
}