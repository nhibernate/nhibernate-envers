using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Store;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Mapping;

namespace NHibernate.Envers.Configuration.Metadata.Reader
{
	/// <summary>
	/// A helper class to read versioning meta-data from annotations on a persistent class.
	/// </summary>
	public sealed class AnnotationsMetadataReader 
	{
		private readonly IMetaDataStore _metaDataStore;
		private readonly GlobalConfiguration globalCfg;
		private readonly PersistentClass pc;

		/// <summary>
		/// This object is filled with information read from annotations and returned by the <code>getVersioningData</code>
		/// method.
		/// </summary>
		private readonly ClassAuditingData _auditData;

		public AnnotationsMetadataReader(IMetaDataStore metaDataStore,
									GlobalConfiguration globalCfg, 
									PersistentClass pc)
		{
			_metaDataStore = metaDataStore;
			this.globalCfg = globalCfg;
			this.pc = pc;

			_auditData = new ClassAuditingData();
		}

		public ClassAuditingData GetAuditData()
		{
			if (pc.ClassName == null)
			{
				return _auditData;
			}

			var typ = pc.MappedClass;

			if (defaultAudited(typ))
			{
				_auditData.SetDefaultAudited(true);
			}

			var ar = new AuditedPropertiesReader(_metaDataStore, 
													new PersistentClassPropertiesSource(typ, this), _auditData,
													globalCfg, string.Empty);
			ar.Read();

			addAuditTable(typ);
			addAuditSecondaryTables(typ);
			addFactory(typ);

			return _auditData;
		}

		private bool defaultAudited(System.Type typ) 
		{
			var defaultAudited = _metaDataStore.ClassMeta<AuditedAttribute>(typ);

			return defaultAudited != null;
		}

		private void addAuditTable(System.Type typ)
		{
			var auditTable = _metaDataStore.ClassMeta<AuditTableAttribute>(typ);
			_auditData.AuditTable = auditTable ?? getDefaultAuditTable();
		}

		private void addAuditSecondaryTables(System.Type typ)
		{
			IEntityMeta entityMeta;
			if (_metaDataStore.EntityMetas.TryGetValue(typ, out entityMeta))
			{
				var joinAuditTableAttributes = entityMeta.ClassMetas.OfType<JoinAuditTableAttribute>().ToList();
				foreach (var joinAuditTableAttribute in joinAuditTableAttributes)
				{
					_auditData.JoinTableDictionary.Add(joinAuditTableAttribute.JoinTableName, joinAuditTableAttribute.JoinAuditTableName);
				}
			}
		}

		private void addFactory(System.Type typ)
		{
			var factory = _metaDataStore.ClassMeta<AuditFactoryAttribute>(typ);
			_auditData.Factory = factory ?? getDefaultFactory();
		}

		private readonly AuditFactoryAttribute defaultFactory = new AuditFactoryAttribute(new DefaultAuditEntityFactory());
		private AuditFactoryAttribute getDefaultFactory()
		{
			return defaultFactory;
		}

		private readonly AuditTableAttribute defaultAuditTable = new AuditTableAttribute(string.Empty);
		  

		private AuditTableAttribute getDefaultAuditTable() 
		{
			return defaultAuditTable;
		}

		private class PersistentClassPropertiesSource : IPersistentPropertiesSource 
		{
			private readonly AnnotationsMetadataReader parent;

			public PersistentClassPropertiesSource(System.Type typ, AnnotationsMetadataReader parent) 
			{ 
				this.parent = parent;
				DeclaredPersistentProperties = PropertyAndMemberInfo.PersistentInfo(typ, parent.pc.PropertyIterator, true);
				Class = typ;
			}

			public IEnumerable<DeclaredPersistentProperty> DeclaredPersistentProperties { get; private set; }
			public System.Type Class { get; private set; }

			public Property VersionedProperty
			{
				get { return parent.pc.Version; }
			}

			public bool IsComponent
			{
				get { return false; }
			}
		}
	}
}
