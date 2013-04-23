using System.Linq;
using NHibernate.Mapping;

namespace NHibernate.Envers.Configuration.Metadata
{
	public enum InheritanceType
	{
			None,
			Joined,
			Single,
			TablePerClass
	}

	public static class InheritanceTypeExtensions
	{
		/// <summary>
		/// Get the <see cref="InheritanceType"/> for a given <see cref="PersistentClass"/>.
		/// </summary>
		/// <param name="source">The class for which to get the inheritance type.</param>
		/// <returns>
		/// The inheritance type of this class. NONE, if this class does not inherit from another persisten class.
		/// </returns>
		public static InheritanceType GetInheritanceType(this PersistentClass source)
		{
			var superclass = source.Superclass;
			// We assume that every subclass is of the same type.
			return superclass == null ? 
				InheritanceType.None : doGetForSubclass(superclass.SubclassIterator.FirstOrDefault());
		}

		private static InheritanceType doGetForSubclass(Subclass subclass)
		{
			if (subclass is SingleTableSubclass)
			{
				return InheritanceType.Single;
			}
			if (subclass is JoinedSubclass)
			{
				return InheritanceType.Joined;
			}
			if (subclass is UnionSubclass)
			{
				return InheritanceType.TablePerClass;
			}

			throw new MappingException("Unknown subclass class: " + (subclass != null ? subclass.GetType().FullName : "Not available type."));
		}
	}
}
