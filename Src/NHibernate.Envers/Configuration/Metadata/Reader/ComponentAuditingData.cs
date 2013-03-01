using System.Collections.Generic;

namespace NHibernate.Envers.Configuration.Metadata.Reader
{
	public class ComponentAuditingData : PropertyAuditingData, IAuditedPropertiesHolder 
	{
		private readonly IDictionary<string, PropertyAuditingData> properties;

		public ComponentAuditingData() 
		{
			properties = new Dictionary<string, PropertyAuditingData>();
		}

		public void AddPropertyAuditingData(string propertyName, PropertyAuditingData auditingData) 
		{
			properties.Add(propertyName, auditingData);
		}

		public PropertyAuditingData GetPropertyAuditingData(string propertyName)
		{
			PropertyAuditingData ret;
			properties.TryGetValue(propertyName, out ret);
			return ret;
		}

		public bool IsEmpty()
		{
			return properties.Count == 0;
		}

		public bool Contains(string propertyName)
		{
			return properties.ContainsKey(propertyName);
		}

		public IEnumerable<string> PropertyNames
		{
			get { return properties.Keys; }
		}
	}
}
