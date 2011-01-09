using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping;

namespace NHibernate.Envers.Configuration.Metadata.Reader
{
	/**
	 * A helper class to read versioning meta-data from annotations on a persistent class.
	 * @author Simon Duduica, port of Envers Tools class by Adam Warski (adam at warski dot org)
	 * @author Sebastian Komander
	 */
	public sealed class AnnotationsMetadataReader {
		private GlobalConfiguration globalCfg;
		private PersistentClass pc;

		/**
		 * This object is filled with information read from annotations and returned by the <code>getVersioningData</code>
		 * method.
		 */
		private ClassAuditingData _auditData;
		public ClassAuditingData AuditData {
			get{
				if (pc.ClassName == null) {
					return _auditData;
				}
				try {
					System.Type typ = System.Type.GetType(pc.ClassName, true);
					//XClass xclass = reflectionManager.classForName(pc.ClassName, this.GetType());

					ModificationStore defaultStore = getDefaultAudited(typ);
					if (defaultStore != ModificationStore._NULL) {
						_auditData.setDefaultAudited(true);
					}

					AuditedPropertiesReader ar = new AuditedPropertiesReader(defaultStore, new PersistentClassPropertiesSource(typ, this), _auditData,
							globalCfg, "");
					ar.read();

					addAuditTable(typ);
					addAuditSecondaryTables(typ);
				}
				catch (System.Exception e)
				{
					throw new MappingException(e);
				}


				return _auditData;
			}
		}

		public AnnotationsMetadataReader(GlobalConfiguration globalCfg, PersistentClass pc) {
			this.globalCfg = globalCfg;
			this.pc = pc;

			_auditData = new ClassAuditingData();
		}

		private ModificationStore getDefaultAudited(System.Type typ) {
			AuditedAttribute defaultAudited = (AuditedAttribute)Attribute.GetCustomAttribute(typ, typeof(AuditedAttribute));
			//AuditedAttribute defaultAudited = typ.GetCustomAttributes(typeof(AuditedAttribute),);

			if (defaultAudited != null) {
				return defaultAudited.ModStore;
			} else {
				return ModificationStore._NULL;
			}
		}

		private void addAuditTable(System.Type typ) {
			AuditTableAttribute auditTable = (AuditTableAttribute)Attribute.GetCustomAttribute(typ, typeof(AuditTableAttribute));
			//AuditTableAttribute auditTable = clazz.getAnnotation(AuditTable.class);
			if (auditTable != null) {
				_auditData.AuditTable = auditTable;
			} else {
				_auditData.AuditTable = getDefaultAuditTable();
			}
		}

		private void addAuditSecondaryTables(System.Type typ) {
			// Getting information on secondary tables
			//SecondaryAuditTableAttribute secondaryVersionsTable1 = clazz.getAnnotation(SecondaryAuditTable.class);
			SecondaryAuditTableAttribute secondaryVersionsTable1 = (SecondaryAuditTableAttribute)Attribute.GetCustomAttribute(typ, typeof(SecondaryAuditTableAttribute));
			if (secondaryVersionsTable1 != null) {
				_auditData.SecondaryTableDictionary.Add(secondaryVersionsTable1.secondaryTableName,
						secondaryVersionsTable1.secondaryAuditTableName);
			}

			//SecondaryAuditTablesAttribute secondaryAuditTables = clazz.getAnnotation(SecondaryAuditTables.class);
			SecondaryAuditTablesAttribute secondaryAuditTables = (SecondaryAuditTablesAttribute)Attribute.GetCustomAttribute(typ, typeof(SecondaryAuditTablesAttribute));
			if (secondaryAuditTables != null) {
				foreach (SecondaryAuditTableAttribute secondaryAuditTable2 in secondaryAuditTables.Value) {
					_auditData.SecondaryTableDictionary.Add(secondaryAuditTable2.secondaryTableName,
							secondaryAuditTable2.secondaryAuditTableName);
				}
			}
		}



		private readonly AuditTableAttribute defaultAuditTable = new DefaultAuditTableAttribute(string.Empty) { Schema = string.Empty, Catalog = string.Empty};
		  

		private AuditTableAttribute getDefaultAuditTable() {
			return defaultAuditTable;
		}

		private class PersistentClassPropertiesSource : IPersistentPropertiesSource {
			private System.Type typ;
			private AnnotationsMetadataReader parent;

			public PersistentClassPropertiesSource(System.Type typ, AnnotationsMetadataReader parent) 
			{ 
				this.typ = typ; 
				this.parent = parent;
			}

			//@SuppressWarnings({"unchecked"})
			public IEnumerable<Property> PropertyEnumerator {get { return parent.pc.PropertyIterator;}}
			public Property GetProperty(String propertyName) { return parent.pc.GetProperty(propertyName); }
			public System.Type GetClass() { return typ; }
		}
	}
}
