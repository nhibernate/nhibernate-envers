using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers.Entities
{
    /**
     * Type of a relation between two entities.
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
    */
    public enum RelationType
    {
        /**
         * A single-reference-valued relation. The entity owns the relation.
         */
        TO_ONE,
        /**
         * A single-reference-valued relation. The entity doesn't own the relation. It is directly mapped in the related
         * entity.
         */
        TO_ONE_NOT_OWNING,
        /**
         * A collection-of-references-valued relation. The entity doesn't own the relation. It is directly mapped in the
         * related entity.
         */
        TO_MANY_NOT_OWNING,
        /**
         * A collection-of-references-valued relation. The entity owns the relation. It is mapped using a middle table.
         */
        TO_MANY_MIDDLE,
        /**
         * A collection-of-references-valued relation. The entity doesn't own the relation. It is mapped using a middle
         * table.
         */
        TO_MANY_MIDDLE_NOT_OWNING
    }
}
