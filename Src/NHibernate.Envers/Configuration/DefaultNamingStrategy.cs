using System;
using NHibernate.Mapping;

namespace NHibernate.Envers.Configuration
{
	[Serializable]
	public class DefaultNamingStrategy : IEnversNamingStrategy
	{
		public void Initialize(string defaultPrefix, string defaultSuffix)
		{
			DefaultPrefix = defaultPrefix;
			DefaultSuffix = defaultSuffix;
		}

		protected string DefaultPrefix { get; private set; }
		protected string DefaultSuffix { get; private set; }

		public virtual string JoinTableName(Join originalJoin)
		{
			return string.Concat(DefaultPrefix, originalJoin.Table.Name, DefaultSuffix);
		}

		public virtual string AuditTableName(PersistentClass persistentClass)
		{
			return string.Concat(DefaultPrefix, persistentClass.Table.Name, DefaultSuffix);
		}

		public virtual string UnidirectionOneToManyTableName(PersistentClass referencingPersistentClass, PersistentClass referencedPersistentClass)
		{
			var referencedEntityName = referencedPersistentClass.EntityName;
			var referencingEntityName = referencingPersistentClass.EntityName;
			return string.Concat(DefaultPrefix,
			                     referencedEntityName.Substring(referencedEntityName.LastIndexOf(".") + 1),
			                     "_",
			                     referencingEntityName.Substring(referencingEntityName.LastIndexOf(".") + 1),
			                     DefaultSuffix);
		}

		public virtual string ReferenceTableName(Mapping.Collection originalCollection)
		{
			return string.Concat(DefaultPrefix, originalCollection.CollectionTable.Name, DefaultSuffix);
		}
	}
}