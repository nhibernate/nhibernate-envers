using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers.Tools
{
    /**
     * Generates metadata for to-one relations (reference-valued properties).
     * @author @author Catalina Panait, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public class MutableInteger
    {
        private int value;

        public MutableInteger()
        {
        }

        public int getAndIncrease()
        {
            return value++;
        }
    }
}
