using NHibernate.Mapping;

namespace NHibernate.Envers.Configuration
{
	/// <summary>
	/// Give names to envers auditing tables.
	/// Setting explicit names to tables will override these default namings.
	/// </summary>
	public interface IEnversNamingStrategy
	{
		/// <summary>
		/// Called once at start up.
		/// </summary>
		/// <param name="defaultPrefix">
		/// The <see cref="ConfigurationKey.AuditTablePrefix"/> set by user.
		/// </param>
		/// <param name="defaultSuffix">
		/// The <see cref="ConfigurationKey.AuditTableSuffix"/> set by user.
		/// </param>
		void Initialize(string defaultPrefix, string defaultSuffix);

		/// <summary>
		/// Audit table name for a <![CDATA[<join>]]> mapping.
		/// </summary>
		string JoinTableName(Join originalJoin);

		/// <summary>
		/// Audit table name for an entity
		/// </summary>
		string AuditTableName(PersistentClass persistentClass);

		/// <summary>
		/// Audit join table name for a non inversed <![CDATA[<one-to-many>]]> mapping.
		/// </summary>
		string UnidirectionOneToManyTableName(PersistentClass referencingPersistentClass, PersistentClass referencedPersistentClass);

		/// <summary>
		/// Audit table name for a reference mapping (not inversed <![CDATA[<one-to-many>]]> mapping).
		/// </summary>
		string CollectionTableName(Mapping.Collection originalCollection);
	}
}