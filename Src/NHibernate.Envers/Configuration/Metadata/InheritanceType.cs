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
			None,
			Joined,
			Single,
			TablePerClass
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
				return Type.None;
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
				return Type.Single;
			}
			if (subclass is JoinedSubclass) 
			{
				return Type.Joined;
			}
			if (subclass is UnionSubclass) 
			{
				return Type.TablePerClass;
			}

			throw new MappingException("Unknown subclass class: " + subclass.ClassName);
		}
	}
}
