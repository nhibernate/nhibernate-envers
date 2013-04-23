using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Envers.Configuration.Metadata.Reader;
using NHibernate.Mapping;
using NHibernate.Properties;

namespace NHibernate.Envers.Tools.Reflection
{
	public static class PropertyAndMemberInfo
	{
		private const BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.FlattenHierarchy;

		private static readonly List<IFieldNamingStrategy> DefaultFieldNamningStrategies =
			new List<IFieldNamingStrategy>
				{
					new CamelCaseStrategy(),
					new CamelCaseUnderscoreStrategy(),
					new LowerCaseStrategy(),
					new LowerCaseUnderscoreStrategy(),
					new PascalCaseUnderscoreStrategy(),
					new PascalCaseMUnderscoreStrategy(),
				};

		public static IEnumerable<DeclaredPersistentProperty> PersistentInfo(System.Type @class, IEnumerable<Property> properties)
		{
			return PersistentInfo(@class, properties, false);
		}


		public static IEnumerable<DeclaredPersistentProperty> PersistentInfo(System.Type @class, IEnumerable<Property> properties, bool includeEmbedded)
		{
			// a persistent property can be anything including a noop "property" declared in the mapping
			// for query only. In this case I will apply some trick to get the MemberInfo.
			var candidateMembers =
				@class.GetFields(DefaultBindingFlags).Concat(@class.GetProperties(DefaultBindingFlags).Cast<MemberInfo>()).ToList();
			var candidateMembersNames = candidateMembers.Select(m => m.Name).ToList();
			foreach (var property in properties)
			{
				if(includeEmbedded && property.PropertyAccessorName=="embedded")
					yield return new DeclaredPersistentProperty(property, DeclaredPersistentProperty.NotAvailableMemberInfo);
				var exactMemberIdx = candidateMembersNames.IndexOf(property.Name);
				if (exactMemberIdx >= 0)
				{
					// No metter which is the accessor the audit-attribute should be in the property where available and not
					// to the member used to read-write the value. (This method work even for access="field").
					yield return new DeclaredPersistentProperty(property, candidateMembers[exactMemberIdx]);
				}
				else
				{
					// try to find the field using field-name-strategy
					//
					// This part will run for:
					// 1) query only property (access="none" or access="noop")
					// 2) a strange case where the element <property> is declared with a "field.xyz" but only a field is used in the class. (Only God may know way)
					var exactFieldIdx = getMemberIdxByFieldNamingStrategies(candidateMembersNames, property);
					if (exactFieldIdx >= 0)
					{
						yield return new DeclaredPersistentProperty(property, candidateMembers[exactFieldIdx]);
					}
				}
			}
		}

		private static int getMemberIdxByFieldNamingStrategies(IList<string> candidateMembersNames, Property property)
		{
			var exactFieldIdx = -1;
			foreach (var ns in DefaultFieldNamningStrategies)
			{
				var fieldName = ns.GetFieldName(property.Name);
				exactFieldIdx = candidateMembersNames.IndexOf(fieldName);
				if (exactFieldIdx >= 0)
				{
					break;
				}
			}
			return exactFieldIdx;
		}
	}
}