using NHibernate.Envers.Configuration;

namespace NHibernate.Envers.Query.Property
{
	/// <summary>
	/// Provides a function to get the name of a property, which is used in a query, to apply some restrictions on it.
	/// </summary>
	public interface IPropertyNameGetter
	{
		/// <summary>
		/// </summary>
		/// <param name="auditCfg">Audit configuration.</param>
		/// <returns>Name of the property, to be used in a query.</returns>
		string Get(AuditConfiguration auditCfg);
	}
}
