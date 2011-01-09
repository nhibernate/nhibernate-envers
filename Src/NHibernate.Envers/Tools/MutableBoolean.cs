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
    public class MutableBoolean
    {
        private bool value;

        public MutableBoolean()
        {
        }

        public MutableBoolean(bool value)
        {
            this.value = value;
        }

        public bool isSet()
        {
            return value;
        }

        public void set()
        {
            value = true;
        }

        public void unset()
        {
            value = false;
        }
    }
}
