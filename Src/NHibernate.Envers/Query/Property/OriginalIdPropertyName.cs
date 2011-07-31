using NHibernate.Envers.Configuration;

namespace NHibernate.Envers.Query.Property
{
	/// <summary>
	/// Used for specifying restrictions on the identifier.
	/// TODO: idPropertyName should be read basing on auditCfg + entityName
	/// </summary>
	public class OriginalIdPropertyName : IPropertyNameGetter
	{
		private readonly string idPropertyName;

		public OriginalIdPropertyName(string idPropertyName)
		{
			this.idPropertyName = idPropertyName;
		}

		public string Get(AuditConfiguration auditCfg)
		{
			return auditCfg.AuditEntCfg.OriginalIdPropName + "." + idPropertyName;
		}
	}
}
