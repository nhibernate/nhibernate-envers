using System.Collections.Generic;
using NHibernate.Engine;

namespace NHibernate.Envers.Entities.Mapper
{
	public interface IExtendedPropertyMapper: IPropertyMapper, ICompositeMapperBuilder 
	{
		bool Map(ISessionImplementor session, IDictionary<string, object> data, string[] propertyNames, object[] newState, object[] oldState);
	}
}
