using System;
using System.Collections.Generic;
using System.Xml;
using NHibernate.Envers.Configuration.Metadata;
using NHibernate.Envers.Tools.Graph;
using NHibernate.Envers.Entities;
using NHibernate.Mapping;
using NHibernate.Envers.Configuration.Metadata.Reader;

namespace NHibernate.Envers.Configuration
{
	public class EntitiesConfigurator 
	{
		public EntitiesConfigurations Configure(Cfg.Configuration cfg, 
												GlobalConfiguration globalCfg, AuditEntitiesConfiguration verEntCfg,
												XmlDocument revisionInfoXmlMapping, XmlElement revisionInfoRelationMapping) 
		{
			// Creating a name register to capture all audit entity names created.
			var auditEntityNameRegister = new AuditEntityNameRegister();
			
			// Sorting the persistent class topologically - superclass always before subclass
			var classes = GraphTopologicalSort.Sort(new PersistentClassGraphDefiner(cfg));

			var classesAuditingData = new ClassesAuditingData();
			var xmlMappings = new Dictionary<PersistentClass, EntityXmlMappingData>();

			// Reading metadata from annotations
			foreach (var pc in classes)
			{
				// Collecting information from annotations on the persistent class pc
				var annotationsMetadataReader = new AnnotationsMetadataReader(globalCfg, pc);
				var auditData = annotationsMetadataReader.AuditData;

				classesAuditingData.AddClassAuditingData(pc, auditData);
			}

			// Now that all information is read we can update the calculated fields.
			classesAuditingData.UpdateCalculatedFields();

			var auditMetaGen = new AuditMetadataGenerator(cfg, globalCfg, verEntCfg,
					revisionInfoRelationMapping, auditEntityNameRegister, classesAuditingData);

			// First pass
			foreach (var pcDatasEntry in classesAuditingData.GetAllClassAuditedData())
			{
				var pc = pcDatasEntry.Key;
				var auditData = pcDatasEntry.Value;

				var xmlMappingData = new EntityXmlMappingData();
				if (auditData.IsAudited()) 
				{
					if (!string.IsNullOrEmpty(auditData.AuditTable.Value))
					{ 
						verEntCfg.AddCustomAuditTableName(pc.EntityName, auditData.AuditTable.Value);
					}

					auditMetaGen.GenerateFirstPass(pc, auditData, xmlMappingData, true);
				} 
				else 
				{
					auditMetaGen.GenerateFirstPass(pc, auditData, xmlMappingData, false);
				}

				xmlMappings.Add(pc, xmlMappingData);
			}

			// Second pass
			foreach (var pcDatasEntry in classesAuditingData.GetAllClassAuditedData())
			{
				var xmlMappingData = xmlMappings[pcDatasEntry.Key];

				if (pcDatasEntry.Value.IsAudited()) 
				{
					auditMetaGen.GenerateSecondPass(pcDatasEntry.Key, pcDatasEntry.Value, xmlMappingData);

					cfg.AddDocument(xmlMappingData.MainXmlMapping);

					foreach (XmlDocument additionalMapping in  xmlMappingData.AdditionalXmlMappings) 
					{
						cfg.AddDocument(additionalMapping);
					}
				}
			}

			// Only if there are any versioned classes
			if (classesAuditingData.GetAllClassAuditedData().Count > 0)
			{
				if (revisionInfoXmlMapping !=  null) 
				{
					cfg.AddDocument((revisionInfoXmlMapping));
				}
			}

			return new EntitiesConfigurations(auditMetaGen.EntitiesConfigurations,
					auditMetaGen.NotAuditedEntitiesConfigurations);
		}
	}
}
