using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Query.Property
{
	/// <summary>
	/// Used for specifying restrictions on a property of an audited entity.
	/// </summary>
	public class EntityPropertyName : IPropertyNameGetter 
	{
		private readonly string propertyName;

		public EntityPropertyName(string propertyName) 
		{
			this.propertyName = propertyName;
		}

		public string Get(AuditConfiguration auditCfg) 
		{
			return propertyName.Replace(".", MappingTools.RelationCharacter);
		}
	}
}
