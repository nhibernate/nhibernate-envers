using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping;

namespace NHibernate.Envers.Configuration.Metadata
{
    /// <summary>
    /// @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
    /// </summary>
    class InheritanceType
    {
        public enum Type{
            NONE,
            JOINED,
            SINGLE,
            TABLE_PER_CLASS
        }
        //public static readonly InheritanceType NONE = new InheritanceType(0);
        //public static readonly InheritanceType JOINED = new InheritanceType(1);
        //public static readonly InheritanceType SINGLE = new InheritanceType(2);
        //public static readonly InheritanceType TABLE_PER_CLASS = new InheritanceType(3);

        /**
         * @param pc The class for which to get the inheritance type.
         * @return The inheritance type of this class. NONE, if this class does not inherit from
         * another persisten class.
         */
        public static InheritanceType.Type GetForChild(PersistentClass pc) {
            PersistentClass superclass = pc.Superclass;
            if (superclass == null) {
                return InheritanceType.Type.NONE;
            }

            // We assume that every subclass is of the same type.
            IEnumerator<Subclass> enu = superclass.SubclassIterator.GetEnumerator();
            enu.MoveNext();
            return DoGetForSubclass(enu.Current);
        }

	    public static InheritanceType.Type GetForParent(PersistentClass pc) {
            IEnumerator<Subclass> enu = pc.SubclassIterator.GetEnumerator();
		    if (!enu.MoveNext()) {
			    return InheritanceType.Type.NONE;
		    }

		    // We assume that every subclass is of the same type.
            return DoGetForSubclass(enu.Current);
	    }

	    private static InheritanceType.Type DoGetForSubclass(Subclass subclass) {
		    if (subclass is SingleTableSubclass) {
                return InheritanceType.Type.SINGLE;
            } else if (subclass is JoinedSubclass) {
                return InheritanceType.Type.JOINED;
            } else if (subclass is UnionSubclass) {
                return InheritanceType.Type.TABLE_PER_CLASS;
            }

            throw new MappingException("Unknown subclass class: " + subclass.ClassName);
	    }
    }
}
