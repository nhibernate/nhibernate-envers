using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers
{
    /**
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public enum ModificationStore
    {
        _NULL, // C# does not accept null values passed for an enum value so we use this.
        FULL
    }
}
