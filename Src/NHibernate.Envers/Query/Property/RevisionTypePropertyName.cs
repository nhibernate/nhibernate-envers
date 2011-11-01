using NHibernate.Envers.Configuration;

namespace NHibernate.Envers.Query.Property
{
	/// <summary>
	/// Used for specifying restrictions on the revision type, corresponding to an audit entity.
	/// </summary>
	public class RevisionTypePropertyName : IPropertyNameGetter
	{
		public string Get(AuditConfiguration auditCfg)
		{
			return auditCfg.AuditEntCfg.RevisionTypePropName;
		}
	}
}
