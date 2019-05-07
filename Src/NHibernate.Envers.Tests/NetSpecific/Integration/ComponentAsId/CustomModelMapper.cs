using NHibernate.Mapping.ByCode;
using System;
using System.Collections.Generic;
using System.Text;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.ComponentAsId
{
	public class CustomModelMapper : ConventionModelMapper
	{
		public CustomModelMapper()
		{
			IsEntity((t, declared) =>
			{
				return (declared || ((typeof(Entity<>) != t)
									&& !t.IsInterface
									&& !t.IsGenericType
									&& !t.IsAbstract
									&& t.IsSubclassOf(typeof(Entity<>))
									));
			});

			IsRootEntity((t, declared) => declared || (!t.IsInterface && IsFirstConcreteSubclassOfGeneric(typeof(Entity<>), t)));

			IsPersistentProperty((member, declared) => declared || !MappingConvention.IsMemberNotMapped(member));
		}

		public static bool IsFirstConcreteSubclassOfGeneric(System.Type generic, System.Type toCheck)
		{
			if (toCheck == null) return false;
			if (toCheck.IsGenericType || (toCheck == typeof(object))) return false;
			toCheck = toCheck.BaseType;

			while (toCheck != typeof(object))
			{
				if (!toCheck.IsGenericType) return false;
				toCheck = toCheck.GetGenericTypeDefinition();
				if (generic == toCheck)
				{
					return true;
				}
				toCheck = toCheck.BaseType;
			}
			return false;
		}
	}
}
