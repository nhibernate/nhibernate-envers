using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Query;
using NHibernate.Mapping;

namespace NHibernate.Envers.Tests.Tools
{
	public static class TestTools
	{
		public static IEnumerable<int> ExtractRevisionNumbersFromHistory<TEntity>(
			this IEnumerable<IRevisionEntityInfo<TEntity, DefaultRevisionEntity>> org)
		{
			return org.Select(revisionEntityInfo => revisionEntityInfo.RevisionEntity.Id).ToList();
		}

		public static IEnumerable<int> ExtractRevisionNumbersFromRevision(this IList org)
		{
			return (from IList item in org select (DefaultRevisionEntity) item[1] into revEntity select revEntity.Id).ToList();
		}

		public static IEnumerable<string> ExtractModProperties(this PersistentClass pc)
		{
			return pc.ExtractModProperties("_MOD");
		}

		public static IEnumerable<string> ExtractModProperties(this PersistentClass pc, string suffix)
		{
			var result = new HashSet<string>();
			foreach (var property in pc.PropertyIterator)
			{
				var propertyName = property.Name;
				if (propertyName.EndsWith(suffix))
					result.Add(propertyName);
			}
			return result;
		}
	}
}