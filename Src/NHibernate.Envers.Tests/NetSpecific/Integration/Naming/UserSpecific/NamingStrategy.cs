using NHibernate.Envers.Configuration;
using NHibernate.Mapping;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Naming.UserSpecific
{
	public class NamingStrategy : IEnversNamingStrategy
	{
		private string _pre;
		private string _suf;

		public void Initialize(string defaultPrefix, string defaultSuffix)
		{
			_pre = defaultPrefix;
			_suf = defaultSuffix;
		}

		public string JoinTableName(Mapping.Join originalJoin)
		{
			return originalJoin.Table.Name + "2";
		}

		public string AuditTableName(PersistentClass persistentClass)
		{
			return _pre + persistentClass.Table.Name + _suf;
		}

		public string UnidirectionOneToManyTableName(PersistentClass referencingPersistentClass, PersistentClass referencedPersistentClass)
		{
			return referencingPersistentClass.Table.Name + referencedPersistentClass.Table.Name + "3";
		}

		public string CollectionTableName(Mapping.Collection originalCollection)
		{
			return originalCollection.CollectionTable.Name + "4";
		}
	}
}