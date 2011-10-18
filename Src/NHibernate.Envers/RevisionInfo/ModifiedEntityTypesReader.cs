using Iesi.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Tools;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Properties;

namespace NHibernate.Envers.RevisionInfo
{
	/// <summary>
	/// Returns modified entity types from a persisted revision info entity.
	/// </summary>
	public class ModifiedEntityTypesReader
	{
		private readonly IGetter modifiedEntityTypesGetter;

		public ModifiedEntityTypesReader(System.Type revisionInfoClass, PropertyData modifiedEntityTypesData)
		{
			modifiedEntityTypesGetter = ReflectionTools.GetGetter(revisionInfoClass, modifiedEntityTypesData);
		}

		public ISet<System.Type> ModifiedEntityTypes(ISessionImplementor sessionImplementor, object revisionEntity)
		{
			// The default mechanism of tracking entity types that have been changed during each revision, stores
			// fully qualified CLR class names.
			var result = new HashedSet<System.Type>();
			var modifiedEntityClassNames = (ISet<string>)modifiedEntityTypesGetter.Get(revisionEntity);
			if (modifiedEntityClassNames != null)
			{
				foreach (var entityClassName in modifiedEntityClassNames)
				{
					result.Add(Toolz.ResolveEntityClass(sessionImplementor, entityClassName));
				}
			}
			return result;
		}
	}
}