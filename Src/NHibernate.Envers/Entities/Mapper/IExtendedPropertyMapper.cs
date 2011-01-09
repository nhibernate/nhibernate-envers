using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Engine;

namespace NHibernate.Envers.Entities.Mapper
{
    /**
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public interface IExtendedPropertyMapper: IPropertyMapper, ICompositeMapperBuilder {
        bool Map(ISessionImplementor session, IDictionary<String, Object> data, String[] propertyNames, Object[] newState, Object[] oldState);
    }
}
