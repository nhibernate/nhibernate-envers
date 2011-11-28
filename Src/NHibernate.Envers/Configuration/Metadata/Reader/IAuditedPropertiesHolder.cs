namespace NHibernate.Envers.Configuration.Metadata.Reader
{
	/// <summary>
	/// Implementations hold other audited properties.
	/// </summary>
	public interface IAuditedPropertiesHolder
	{
		/// <summary>
		/// Add an audited property.
		/// </summary>
		/// <param name="propertyName">Name of the audited property.</param>
		/// <param name="auditingData">Data for the audited property.</param>
		void AddPropertyAuditingData(string propertyName, PropertyAuditingData auditingData);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName">Name of a property.</param>
		/// <returns>Auditing data for the property.</returns>
		PropertyAuditingData GetPropertyAuditingData(string propertyName);

		bool IsEmpty();

		bool Contains(string propertyName);
	}
}
