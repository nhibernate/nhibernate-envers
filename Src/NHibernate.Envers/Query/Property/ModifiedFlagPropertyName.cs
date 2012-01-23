using NHibernate.Envers.Configuration;
using NHibernate.Envers.Configuration.Metadata;

namespace NHibernate.Envers.Query.Property
{
	public class ModifiedFlagPropertyName : IPropertyNameGetter 
	{
		private readonly IPropertyNameGetter _propertyNameGetter;

		public ModifiedFlagPropertyName(IPropertyNameGetter propertyNameGetter)
		{
			_propertyNameGetter = propertyNameGetter;
		}

		public string Get(AuditConfiguration auditCfg)
		{
			return MetadataTools.ModifiedFlagPropertyName(_propertyNameGetter.Get(auditCfg), auditCfg.GlobalCfg.ModifiedFlagSuffix);
		}
	}
}