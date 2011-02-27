using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping;

namespace NHibernate.Envers.Tools
{
	public static class MappingTools
	{
		/// <summary>
		/// </summary>
		/// <param name="componentName">Name of the component, that is, 
		/// name of the property in the entity that references the component</param>
		/// <returns>A prefix for properties in the given component.</returns>
		public static string CreateComponentPrefix(string componentName)
		{
			return componentName + "_";
		}

		/// <summary>
		/// </summary>
		/// <param name="referencePropertyName">The name of the property that holds the relation to the entity.</param>
		/// <returns>A prefix which should be used to prefix an id mapper for the related entity.</returns>
		public static string CreateToOneRelationPrefix(string referencePropertyName)
		{
			return referencePropertyName + "_";
		}

		public static string ReferencedEntityName(IValue value) 
		{
			if (value is ToOne) 
			{
				return ((ToOne) value).ReferencedEntityName;
			}
			if (value is OneToMany) 
			{
				return ((OneToMany) value).ReferencedEntityName;
			}
			if (value is Mapping.Collection) 
			{
				return ReferencedEntityName(((Mapping.Collection)value).Element);
			}

			return null;
		}

		public static bool SameColumns(IEnumerable<ISelectable> first, IEnumerable<ISelectable> second)
		{
			var firstNames = from f in first
							 select ((Column)f).Name;
			var lastNames = from s in second
							select ((Column)s).Name;
			return firstNames.Count() == lastNames.Count() && firstNames.All(f => lastNames.Contains(f));
		}
	}
}
