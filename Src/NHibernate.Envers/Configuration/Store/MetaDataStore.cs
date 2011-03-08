using System;
using System.Collections.Generic;
using System.Reflection;

namespace NHibernate.Envers.Configuration.Store
{
	public class MetaDataStore : IMetaDataStore
	{
		private readonly Cfg.Configuration _nhConfiguration;
		private readonly IMetaDataProvider _metaDataProvider;
		private readonly IEnumerable<IMetaDataAdder> _metaDataAdders;
		private IDictionary<System.Type, IEntityMeta> _entityMetas;

		public MetaDataStore(Cfg.Configuration nhConfiguration, 
							IMetaDataProvider metaDataProvider,
							IEnumerable<IMetaDataAdder> metaDataAdders)
		{
			_nhConfiguration = nhConfiguration;
			_metaDataProvider = metaDataProvider;
			_metaDataAdders = metaDataAdders;
		}

		public IDictionary<System.Type, IEntityMeta> EntityMetas
		{
			get { return _entityMetas ?? (_entityMetas = initializeMetas()); }
		}

		private IDictionary<System.Type, IEntityMeta> initializeMetas()
		{
			var metaData = _metaDataProvider.CreateMetaData(_nhConfiguration);
			foreach (var metaDataAdder in _metaDataAdders)
			{
				metaDataAdder.AddMetaDataTo(metaData);
			}
			return metaData;
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

		public T MemberMeta<T>(MemberInfo member) where T : Attribute
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
	}
}