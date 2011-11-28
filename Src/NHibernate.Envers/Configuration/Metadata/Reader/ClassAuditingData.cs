using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Configuration.Metadata.Reader
{
	public class ClassAuditingData : IAuditedPropertiesHolder 
	{
		private readonly IDictionary<string, PropertyAuditingData> properties;

		public ClassAuditingData()
		{
			properties = new Dictionary<string, PropertyAuditingData>();
			JoinTableDictionary = new Dictionary<string, string>();
		}

		//called SecondaryTableDictionary in Hibernate Envers
		public IDictionary<string, string> JoinTableDictionary { get; private set; }
		public AuditTableAttribute AuditTable { get; set; }

		/// <summary>
		///  True if the class is audited globally (this helps to cover the cases when there are no fields in the class,
		///  but it's still audited).
		/// </summary>
		private bool defaultAudited;

		public void AddPropertyAuditingData(string propertyName, PropertyAuditingData auditingData)
		{
			properties.Add(propertyName, auditingData);
		}

		public PropertyAuditingData GetPropertyAuditingData(string propertyName)
		{
			PropertyAuditingData ret;
			return properties.TryGetValue(propertyName, out ret) ? ret : null;
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

		public void SetDefaultAudited(bool defaultAudited) 
		{
			this.defaultAudited = defaultAudited;
		}

		public bool IsAudited() 
		{
			return defaultAudited || properties.Count > 0;
		}
	}
}
