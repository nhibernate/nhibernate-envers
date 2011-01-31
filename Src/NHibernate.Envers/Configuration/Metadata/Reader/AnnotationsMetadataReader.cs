using System;
using System.Collections.Generic;
using NHibernate.Mapping;

namespace NHibernate.Envers.Configuration.Metadata.Reader
{
	/**
	 * A helper class to read versioning meta-data from annotations on a persistent class.
	 * @author Simon Duduica, port of Envers Tools class by Adam Warski (adam at warski dot org)
	 * @author Sebastian Komander
	 */
	public sealed class AnnotationsMetadataReader 
	{
		private readonly PropertyAndMemberInfo _propertyAndMemberInfo;
		private GlobalConfiguration globalCfg;
		private PersistentClass pc;


		public AnnotationsMetadataReader(PropertyAndMemberInfo propertyAndMemberInfo, 
									GlobalConfiguration globalCfg, 
									PersistentClass pc)
		{
			_propertyAndMemberInfo = propertyAndMemberInfo;
			this.globalCfg = globalCfg;
			this.pc = pc;

			_auditData = new ClassAuditingData();
		}

		/**
		 * This object is filled with information read from annotations and returned by the <code>getVersioningData</code>
		 * method.
		 */
		private ClassAuditingData _auditData;

		public ClassAuditingData GetAuditData()
		{
			if (pc.ClassName == null)
			{
				return _auditData;
			}
			try
			{
				var typ = pc.MappedClass;

				var defaultStore = getDefaultAudited(typ);
				if (defaultStore != ModificationStore._NULL)
				{
					_auditData.SetDefaultAudited(true);
				}

				var ar = new AuditedPropertiesReader(_propertyAndMemberInfo, defaultStore,
													 new PersistentClassPropertiesSource(typ, this), _auditData,
													 globalCfg, string.Empty);
				ar.Read();

				addAuditTable(typ);
				addAuditSecondaryTables(typ);
			}
			catch (Exception e)
			{
				throw new MappingException(e);
			}


			return _auditData;
		}

		private ModificationStore getDefaultAudited(System.Type typ) 
		{
			var defaultAudited = (AuditedAttribute)Attribute.GetCustomAttribute(typ, typeof(AuditedAttribute), false);

			return defaultAudited != null ? defaultAudited.ModStore : ModificationStore._NULL;
		}

		private void addAuditTable(System.Type typ)
		{
			var auditTable = (AuditTableAttribute)Attribute.GetCustomAttribute(typ, typeof(AuditTableAttribute), false);
			_auditData.AuditTable = auditTable ?? getDefaultAuditTable();
		}

		private void addAuditSecondaryTables(System.Type typ) 
		{
			// Getting information on secondary tables
			//SecondaryAuditTableAttribute secondaryVersionsTable1 = clazz.getAnnotation(SecondaryAuditTable.class);
			var joinVersionsTable1 = (JoinAuditTableAttribute)Attribute.GetCustomAttribute(typ, typeof(JoinAuditTableAttribute), false);
			if (joinVersionsTable1 != null) 
			{
				_auditData.SecondaryTableDictionary.Add(joinVersionsTable1.JoinTableName,
						joinVersionsTable1.JoinAuditTableName);
			}

			var secondaryAuditTables = (SecondaryAuditTablesAttribute)Attribute.GetCustomAttribute(typ, typeof(SecondaryAuditTablesAttribute), false);
			if (secondaryAuditTables != null) 
			{
				foreach (var secondaryAuditTable2 in secondaryAuditTables.Value) 
				{
					_auditData.SecondaryTableDictionary.Add(secondaryAuditTable2.JoinTableName,
							secondaryAuditTable2.JoinAuditTableName);
				}
			}
		}



		private readonly AuditTableAttribute defaultAuditTable = new AuditTableAttribute(string.Empty);
		  

		private AuditTableAttribute getDefaultAuditTable() 
		{
			return defaultAuditTable;
		}

		private class PersistentClassPropertiesSource : IPersistentPropertiesSource 
		{
			private System.Type typ;
			private AnnotationsMetadataReader parent;

			public PersistentClassPropertiesSource(System.Type typ, AnnotationsMetadataReader parent) 
			{ 
				this.typ = typ; 
				this.parent = parent;
			}

			public IEnumerable<Property> PropertyEnumerator 
			{
				get { return parent.pc.PropertyIterator; }
			}

			public System.Type Clazz 
			{ 
				get { return typ; } 
			}

			public Property VersionedProperty
			{
				get { return parent.pc.Version; }
			}
		}
	}
}
