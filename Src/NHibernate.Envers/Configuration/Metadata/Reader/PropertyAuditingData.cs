using System.Collections.Generic;
using NHibernate.Envers.Entities;

namespace NHibernate.Envers.Configuration.Metadata.Reader
{
	/**
	 * @author Adam Warski (adam at warski dot org)
	 */

	public class PropertyAuditingData
	{
		public PropertyAuditingData()
		{
			AuditingOverrides = new List<AuditOverrideAttribute>(0);
		}

		public PropertyAuditingData(string name, string accessType, ModificationStore store,
		                            RelationTargetAuditMode relationTargetAuditMode,
		                            string auditMappedBy, string positionMappedBy,
		                            bool forceInsertable)
		{
			AuditingOverrides = new List<AuditOverrideAttribute>(0);
			Name = name;
			BeanName = name;
			AccessType = accessType;
			Store = store;
			RelationTargetAuditMode = relationTargetAuditMode;
			AuditMappedBy = auditMappedBy;
			PositionMappedBy = positionMappedBy;
			ForceInsertable = forceInsertable;
		}

		public string Name { get; set; }
		public string BeanName { get; set; }
		public ModificationStore Store { get; set; }
		public string MapKey { get; set; }
		public AuditJoinTableAttribute JoinTable { get; set; }
		public string AccessType { get; set; }
		public IList<AuditOverrideAttribute> AuditingOverrides { get; private set; }
		public string AuditMappedBy { get; set; }
		public string PositionMappedBy { get; set; }
		public bool ForceInsertable { get; set; }
		public RelationTargetAuditMode RelationTargetAuditMode { get; set; }

		public PropertyData GetPropertyData()
		{
			return new PropertyData(Name, BeanName, AccessType, Store);
		}

		public void addAuditingOverride(AuditOverrideAttribute annotation)
		{
			if (annotation != null)
			{
				string overrideName = annotation.Name;
				bool present = false;
				foreach (AuditOverrideAttribute current in AuditingOverrides)
				{
					if (current.Name.Equals(overrideName))
					{
						present = true;
						break;
					}
				}
				if (!present)
				{
					AuditingOverrides.Add(annotation);
				}
			}
		}

		public void addAuditingOverrides(AuditOverridesAttribute annotationOverrides)
		{
			if (annotationOverrides != null)
			{
				foreach (AuditOverrideAttribute attrib in annotationOverrides.value)
				{
					addAuditingOverride(attrib);
				}
			}
		}
	}
}