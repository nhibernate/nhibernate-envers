using System.Collections.Generic;
using NHibernate.Mapping;

namespace NHibernate.Envers.Configuration.Metadata
{
	/// <summary>
	/// @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
	/// </summary>
	public static class InheritanceType
	{
		public enum Type
		{
			NONE,
			JOINED,
			SINGLE,
			TABLE_PER_CLASS
		}

		/**
		 * @param pc The class for which to get the inheritance type.
		 * @return The inheritance type of this class. NONE, if this class does not inherit from
		 * another persisten class.
		 */
		public static Type GetForChild(PersistentClass pc) 
		{
			var superclass = pc.Superclass;
			if (superclass == null) 
			{
				return Type.NONE;
			}

			// We assume that every subclass is of the same type.
			var enu = superclass.SubclassIterator.GetEnumerator();
			enu.MoveNext();
			return DoGetForSubclass(enu.Current);
		}

		private static Type DoGetForSubclass(Subclass subclass) 
		{
			if (subclass is SingleTableSubclass) 
			{
				return Type.SINGLE;
			}
			if (subclass is JoinedSubclass) 
			{
				return Type.JOINED;
			}
			if (subclass is UnionSubclass) 
			{
				return Type.TABLE_PER_CLASS;
			}

			throw new MappingException("Unknown subclass class: " + subclass.ClassName);
		}
	}
}
