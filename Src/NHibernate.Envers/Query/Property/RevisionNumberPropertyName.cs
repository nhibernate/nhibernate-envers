using NHibernate.Envers.Configuration;

namespace NHibernate.Envers.Query.Property
{
	/// <summary>
	/// Used for specifying restrictions on the revision number, corresponding to an audit entity.
	/// </summary>
	public class RevisionNumberPropertyName : IPropertyNameGetter
	{
		public string Get(AuditConfiguration auditCfg)
		{
			return auditCfg.AuditEntCfg.RevisionNumberPath;
		}
	}
}
