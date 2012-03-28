using System.Reflection;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Store;

namespace NHibernate.Envers.Configuration.Metadata.Reader
{
	/// <summary>
	/// Reads the audited properties for components.
	/// </summary>
	public class ComponentAuditedPropertiesReader : AuditedPropertiesReader
	{
		private readonly IMetaDataStore _metaDataStore;

		public ComponentAuditedPropertiesReader(IMetaDataStore metaDataStore, 
														IPersistentPropertiesSource persistentPropertiesSource, 
														IAuditedPropertiesHolder auditedPropertiesHolder, 
														GlobalConfiguration globalCfg, 
														string propertyNamePrefix) 
			: base(metaDataStore, persistentPropertiesSource, auditedPropertiesHolder, globalCfg, propertyNamePrefix)
		{
			_metaDataStore = metaDataStore;
		}

		protected override bool CheckAudited(MemberInfo property, string mappedPropertyName, PropertyAuditingData propertyData, AuditedAttribute allClassAudited)
		{
			// Checking if this property is explicitly audited or if all properties are.
			var aud = _metaDataStore.MemberMeta<AuditedAttribute>(property);
			if (aud != null)
			{
				propertyData.RelationTargetAuditMode = aud.TargetAuditMode;
			}
			return true;
		}
	}
}