using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Entities.Mapper.Id;

namespace NHibernate.Envers.Entities.Mapper
{
    /**
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public interface ISimpleMapperBuilder
    {
        void Add(PropertyData propertyData);
    }
}
