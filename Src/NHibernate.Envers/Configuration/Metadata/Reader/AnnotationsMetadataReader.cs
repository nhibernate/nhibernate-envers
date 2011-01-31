using System;
using System.Collections.Generic;
using NHibernate.Envers.Configuration.Store;
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
		private readonly IMetaDataStore _metaDataStore;
		private readonly GlobalConfiguration globalCfg;
		private readonly PersistentClass pc;


		public AnnotationsMetadataReader(PropertyAndMemberInfo propertyAndMemberInfo, 
									IMetaDataStore metaDataStore,
									GlobalConfiguration globalCfg, 
									PersistentClass pc)
		{
			_propertyAndMemberInfo = propertyAndMemberInfo;
			_metaDataStore = metaDataStore;
			this.globalCfg = globalCfg;
			this.pc = pc;

			_auditData = new ClassAuditingData();
		}

		/**
		 * This object is filled with information read from annotations and returned by the <code>getVersioningData</code>
		 * method.
		 */
		private readonly ClassAuditingData _auditData;

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

				var ar = new AuditedPropertiesReader(_propertyAndMemberInfo, _metaDataStore, defaultStore,
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
			var defaultAudited = _metaDataStore.ClassMeta<AuditedAttribute>(typ);

			return defaultAudited != null ? defaultAudited.ModStore : ModificationStore._NULL;
		}

		private void addAuditTable(System.Type typ)
		{
			var auditTable = _metaDataStore.ClassMeta<AuditTableAttribute>(typ);
			_auditData.AuditTable = auditTable ?? getDefaultAuditTable();
		}

		private void addAuditSecondaryTables(System.Type typ) 
		{
			// Getting information on secondary tables
			var joinVersionsTable1 = _metaDataStore.ClassMeta<JoinAuditTableAttribute>(typ);
			if (joinVersionsTable1 != null) 
			{
				_auditData.SecondaryTableDictionary.Add(joinVersionsTable1.JoinTableName,
						joinVersionsTable1.JoinAuditTableName);
			}

			var secondaryAuditTables = _metaDataStore.ClassMeta<SecondaryAuditTablesAttribute>(typ);
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
