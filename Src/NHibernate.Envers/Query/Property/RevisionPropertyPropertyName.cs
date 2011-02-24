using NHibernate.Envers.Configuration;

namespace NHibernate.Envers.Query.Property
{
	/// <summary>
	/// Used for specifying restrictions on a property of the revision entity, which is associated with an audit entity.
	/// </summary>
	public class RevisionPropertyPropertyName : IPropertyNameGetter 
	{
		private readonly string propertyName;

		public RevisionPropertyPropertyName(string propertyName) 
		{
			this.propertyName = propertyName;
		}

		public string Get(AuditConfiguration auditCfg) 
		{
			return auditCfg.AuditEntCfg.GetRevisionPropPath(propertyName);
		}
	}
}
