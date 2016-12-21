using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Entities.Mapper;

namespace NHibernate.Envers.Configuration.Metadata.Reader
{
	public class PropertyAuditingData
	{
		public PropertyAuditingData()
		{
			AuditingOverrides = new List<AuditOverrideAttribute>(0);
		}

		public PropertyAuditingData(string name, string accessType)
		{
			AuditingOverrides = new List<AuditOverrideAttribute>(0);
			Name = name;
			BeanName = name;
			AccessType = accessType;
			RelationTargetAuditMode = RelationTargetAuditMode.Audited;
		}

		public string Name { get; set; }
		public string BeanName { get; set; }
		public AuditJoinTableAttribute JoinTable { get; set; }
		public string AccessType { get; set; }
		public IList<AuditOverrideAttribute> AuditingOverrides { get; }
		public string MappedBy { get; set; }
		public string PositionMappedBy { get; set; }
		public bool ForceInsertable { get; set; }
		public RelationTargetAuditMode RelationTargetAuditMode { get; set; }
		public bool UsingModifiedFlag { get; set; }
		public string ModifiedFlagName { private get; set;}
		public ICustomCollectionMapperFactory CustomCollectionMapperFactory { get; set; }

		public PropertyData GetPropertyData()
		{
			return new PropertyData(Name, BeanName, AccessType, UsingModifiedFlag, ModifiedFlagName);
		}

		public void AddAuditingOverride(AuditOverrideAttribute annotation)
		{
			if (annotation != null)
			{
				var overrideName = annotation.PropertyName;
				var present = AuditingOverrides.Any(current => current.PropertyName.Equals(overrideName));
				if (!present)
				{
					AuditingOverrides.Add(annotation);
				}
			}
		}
	}
}