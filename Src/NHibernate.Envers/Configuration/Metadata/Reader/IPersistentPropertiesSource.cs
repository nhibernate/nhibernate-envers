using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping;

namespace NHibernate.Envers.Configuration.Metadata.Reader
{
    /**
     * A source of data on persistent properties of a class or component.
     * @author Simon Duduica, port of Envers Tools class by Adam Warski (adam at warski dot org)
     */
    public interface IPersistentPropertiesSource
    {
        IEnumerable<Property> PropertyEnumerator { get; }
        Property GetProperty(String propertyName);
        System.Type GetClass();
    }
}
