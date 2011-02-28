using System.Collections.Generic;
using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Configuration.Metadata.Reader
{
	public class ClassAuditingData : IAuditedPropertiesHolder 
	{
		public ClassAuditingData()
		{
			Properties = new Dictionary<string, PropertyAuditingData>();
			JoinTableDictionary = new Dictionary<string, string>();
		}

		public IDictionary<string, PropertyAuditingData> Properties { get; private set; }
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
			Properties.Add(propertyName, auditingData);
		}

		public PropertyAuditingData GetPropertyAuditingData(string propertyName)
		{
			PropertyAuditingData ret;
			return Properties.TryGetValue(propertyName, out ret) ? ret : null;
		}

		public IEnumerable<string> PropertyNames
		{
			get { return Properties.Keys; }
		}

		public void SetDefaultAudited(bool defaultAudited) 
		{
			this.defaultAudited = defaultAudited;
		}

		public bool IsAudited() 
		{
			return defaultAudited || Properties.Count > 0;
		}
	}
}
