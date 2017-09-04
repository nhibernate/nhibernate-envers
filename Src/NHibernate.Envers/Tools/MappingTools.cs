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
		/// Checks if not-found=ignore
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IgnoreNotFound(IValue value)
		{
			if (value is ManyToOne mtoValue)
			{
				return mtoValue.IsIgnoreNotFound;
			}
			if (value is OneToMany otmValue)
			{
				return otmValue.IsIgnoreNotFound;
			}
			return false;
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
			switch (value)
			{
				case ToOne valueToOne:
					return valueToOne.ReferencedEntityName;
				case OneToMany valueOneToMany:
					return valueOneToMany.ReferencedEntityName;
				case Mapping.Collection valueCollection:
					return ReferencedEntityName(valueCollection.Element);
			}

			return null;
		}

		public static bool SameColumns(IEnumerable<ISelectable> first, IEnumerable<ISelectable> second)
		{
			var firstNames = from f in first.OfType<Column>()
							 select f.Name;
			var lastNames = from s in second.OfType<Column>()
							select s.Name;
			return firstNames.Count() == lastNames.Count() && firstNames.All(f => lastNames.Contains(f));
		}

		public static bool AnyColumnMatches(IEnumerable<ISelectable> first, IEnumerable<ISelectable> second)
		{
			return (from Column idColumn in first 
					  from Column propColumn in second 
					  where propColumn.Name.Equals(idColumn.Name) 
					  select idColumn).Any();
		}
	}
}
