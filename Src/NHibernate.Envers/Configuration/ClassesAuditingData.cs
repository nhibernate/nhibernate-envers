using System.Collections.Generic;
using NHibernate.Envers.Configuration.Metadata.Reader;
using NHibernate.Envers.Tools;
using NHibernate.Mapping;

namespace NHibernate.Envers.Configuration
{
	/// <summary>
	/// A helper class holding auditing meta-data for all persistent classes.
	/// </summary>
	public class ClassesAuditingData 
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(ClassesAuditingData));

		private readonly IDictionary<string, ClassAuditingData> entityNameToAuditingData = new Dictionary<string, ClassAuditingData>();
		private readonly IDictionary<PersistentClass, ClassAuditingData> persistentClassToAuditingData = new Dictionary<PersistentClass, ClassAuditingData>();

		/// <summary>
		/// Stores information about auditing meta-data for the given class.
		/// </summary>
		/// <param name="pc">Persistent class.</param>
		/// <param name="cad">Auditing meta-data for the given class.</param>
		public void AddClassAuditingData(PersistentClass pc, ClassAuditingData cad) 
		{
			entityNameToAuditingData.Add(pc.EntityName, cad);
			persistentClassToAuditingData.Add(pc, cad);
		}

		/// <summary>
		/// A collection of all auditing meta-data for persistent classes.
		/// </summary>
		public IEnumerable<KeyValuePair<PersistentClass, ClassAuditingData>> AllClassAuditedData
		{
			get { return persistentClassToAuditingData; }
		}

		/// <summary>
		/// After all meta-data is read, updates calculated fields. This includes:
		/// <ul>
		/// <li>setting {@code forceInsertable} to {@code true} for properties specified by {@code @AuditMappedBy}</li> 
		/// </ul>
		/// </summary>
		public void UpdateCalculatedFields() 
		{
			foreach (var classAuditingDataEntry in persistentClassToAuditingData) 
			{
				var pc = classAuditingDataEntry.Key;
				var classAuditingData = classAuditingDataEntry.Value;
				foreach (var propertyName in classAuditingData.PropertyNames) 
				{
					var propertyAuditingData = classAuditingData.GetPropertyAuditingData(propertyName);
					// If a property had the @AuditMappedBy annotation, setting the referenced fields to be always insertable.
					if (propertyAuditingData.ForceInsertable) 
					{
						var referencedEntityName = MappingTools.ReferencedEntityName(pc.GetProperty(propertyName).Value);

						var referencedClassAuditingData = entityNameToAuditingData[referencedEntityName];

						forcePropertyInsertable(referencedClassAuditingData, propertyAuditingData.MappedBy,
								pc.EntityName, referencedEntityName);

						forcePropertyInsertable(referencedClassAuditingData, propertyAuditingData.PositionMappedBy,
								pc.EntityName, referencedEntityName);
					}
				}
			}
		}

		private static void forcePropertyInsertable(ClassAuditingData classAuditingData, string propertyName,
											 string entityName, string referencedEntityName)
		{
			if (propertyName != null) 
			{
				if (classAuditingData.GetPropertyAuditingData(propertyName) == null) 
				{
					throw new MappingException("@AuditMappedBy points to a property that doesn't exist: " +
						referencedEntityName + "." + propertyName);
				}
				log.DebugFormat("Non-insertable property {0}, {1} will be made insertable because a matching @AuditMappedBy was found in the {2} entity.",
					referencedEntityName, propertyName, entityName);

				classAuditingData
						.GetPropertyAuditingData(propertyName)
						.ForceInsertable = true;
			}
		}
	}
}
