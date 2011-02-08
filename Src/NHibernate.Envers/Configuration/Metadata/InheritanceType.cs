using System.Linq;
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

		/// <summary>
		/// Get the <see cref="InheritanceType.Type"/> for a given <see cref="PersistentClass"/>.
		/// </summary>
		/// <param name="pc">The class for which to get the inheritance type.</param>
		/// <returns>
		/// The inheritance type of this class. NONE, if this class does not inherit from another persisten class.
		/// </returns>
		public static Type GetForChild(PersistentClass pc) 
		{
			var superclass = pc.Superclass;
			if (superclass == null) 
			{
				return Type.None;
			}

			// We assume that every subclass is of the same type.
			return DoGetForSubclass(superclass.SubclassIterator.FirstOrDefault());
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

			throw new MappingException("Unknown subclass class: " + (subclass != null ? subclass.GetType().FullName : "Not available type." ));
		}
	}
}
