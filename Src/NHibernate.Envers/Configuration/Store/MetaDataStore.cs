using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Tools;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Mapping;

namespace NHibernate.Envers.Configuration.Store
{
	public class MetaDataStore : IMetaDataStore
	{
		private readonly Cfg.Configuration _nhConfiguration;
		private readonly IMetaDataProvider _metaDataProvider;
		private IDictionary<System.Type, IEntityMeta> _entityMetas;

		public MetaDataStore(Cfg.Configuration nhConfiguration, IMetaDataProvider metaDataProvider)
		{
			_nhConfiguration = nhConfiguration;
			_metaDataProvider = metaDataProvider;
		}

		public IDictionary<System.Type, IEntityMeta> EntityMetas
		{
			get { return _entityMetas ?? (_entityMetas = initializeMetas()); }
		}

		public T ClassMeta<T>(System.Type entityType) where T : Attribute
		{
			IEntityMeta entityMeta;
			var attrType = typeof (T);
			if (!EntityMetas.TryGetValue(entityType, out entityMeta))
				return null;
			foreach (var enversAttribute in entityMeta.ClassMetas)
			{
				if (enversAttribute.GetType().Equals(attrType))
					return (T)enversAttribute;
			}
			return null;
		}

		public T MemberMeta<T>(MemberInfo member) where T : Attribute //kanske behöver returnera en lista?
		{
			IEntityMeta entityMeta;
			var attrType = typeof(T);
			if (!EntityMetas.TryGetValue(member.ReflectedType, out entityMeta))
				return null;
			IEnumerable<Attribute> attributes;
			if (!entityMeta.MemberMetas.TryGetValue(member, out attributes))
				return null;

			foreach (var enversAttribute in attributes)
			{
				if (enversAttribute.GetType().Equals(attrType))
					return (T)enversAttribute;
			}
			return null;
		}

		public IEnumerable<System.Type> EntitiesDeclaredWith<T>() where T : Attribute
		{
			var ret = new HashSet<System.Type>();
			foreach (var keyValueMeta in EntityMetas)
			{
				foreach (var classMeta in keyValueMeta.Value.ClassMetas)
				{
					if (classMeta.GetType().Equals(typeof(T)))
						ret.Add(keyValueMeta.Key);
				}
			}
			return ret;
		}

		private IDictionary<System.Type, IEntityMeta> initializeMetas()
		{
			var definedMetas = _metaDataProvider.CreateMetaData(_nhConfiguration);
			addBidirectionalInfo(definedMetas);
			return definedMetas;
		}

		private void addBidirectionalInfo(IDictionary<System.Type, IEntityMeta> metas)
		{
			foreach (var type in metas.Keys)
			{
				var persistentClass = _nhConfiguration.GetClassMapping(type);
				if (persistentClass == null) continue;
				foreach (var property in persistentClass.PropertyIterator)
				{
					//is it a collection?
					var collectionValue = property.Value as Mapping.Collection;
					if (collectionValue == null) continue;

					//find referenced entity name
					var referencedEntity = MappingTools.ReferencedEntityName(property.Value);
					if (referencedEntity == null) continue;


					var refPersistentClass = _nhConfiguration.GetClassMapping(referencedEntity);
					foreach (var refProperty in refPersistentClass.PropertyClosureIterator)
					{
						var attr = createAuditMappedByAttributeIfReferenceImmutable(collectionValue, refProperty);
						if (attr == null) continue;
						mightAddIndexToAttribute(attr, collectionValue, refPersistentClass.PropertyClosureIterator);
						var entityMeta = (EntityMeta)metas[type];
						var methodInfo = PropertyAndMemberInfo.PersistentInfo(type, new[] { property }).First().Member;
						entityMeta.AddMemberMeta(methodInfo, attr);
					}
				}
			}
		}

		private static AuditMappedByAttribute createAuditMappedByAttributeIfReferenceImmutable(Mapping.Collection collectionValue, Property referencedProperty)
		{
			//check key value
			if (MappingTools.SameColumns(referencedProperty.ColumnIterator, collectionValue.Key.ColumnIterator))
			{
				if (!referencedProperty.IsUpdateable && !referencedProperty.IsInsertable)
					return new AuditMappedByAttribute { MappedBy = referencedProperty.Name };
			}

			return null;
		}

		private static void mightAddIndexToAttribute(AuditMappedByAttribute auditMappedByAttribute, Mapping.Collection collectionValue, IEnumerable<Property> referencedProperties)
		{
			//check index value
			var indexValue = collectionValue as IndexedCollection;
			if (indexValue == null) return;
			foreach (var referencedProperty in referencedProperties)
			{
				if (MappingTools.SameColumns(referencedProperty.ColumnIterator, indexValue.Index.ColumnIterator) &&
												   !referencedProperty.IsUpdateable &&
												   !referencedProperty.IsInsertable)
				{
					auditMappedByAttribute.PositionMappedBy = referencedProperty.Name;
				}
			}
		}

	}
}