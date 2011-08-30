using System.Collections.Generic;
using System.Linq;
using NHibernate.Mapping;

namespace NHibernate.Envers.Tools
{
	public static class MappingTools
	{
		public const string RelationCharacter = "_";

		/// <summary>
		/// </summary>
		/// <param name="componentName">Name of the component, that is, 
		/// name of the property in the entity that references the component</param>
		/// <returns>A prefix for properties in the given component.</returns>
		public static string CreateComponentPrefix(string componentName)
		{
			return componentName + RelationCharacter;
		}

		/// <summary>
		/// </summary>
		/// <param name="referencePropertyName">The name of the property that holds the relation to the entity.</param>
		/// <returns>A prefix which should be used to prefix an id mapper for the related entity.</returns>
		public static string CreateToOneRelationPrefix(string referencePropertyName)
		{
			return referencePropertyName + RelationCharacter;
		}

		public static string ReferencedEntityName(IValue value)
		{
			var valueToOne = value as ToOne;
			if (valueToOne!=null) 
				return valueToOne.ReferencedEntityName;

			var valueOneToMany = value as OneToMany;
			if (valueOneToMany != null)
				return valueOneToMany.ReferencedEntityName;

			var valueCollection = value as Mapping.Collection;
			if (valueCollection != null)
				return ReferencedEntityName(valueCollection.Element);

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
