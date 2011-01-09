using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers.Entities.Mapper
{
    /**
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public interface ICompositeMapperBuilder : ISimpleMapperBuilder {    
        ICompositeMapperBuilder AddComponent(PropertyData propertyData, String componentClassName);
        void AddComposite(PropertyData propertyData, IPropertyMapper propertyMapper);
    }
}
