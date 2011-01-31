using System;
using System.Collections.Generic;
using System.Reflection;

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
			get { return _entityMetas ?? (_entityMetas = _metaDataProvider.CreateMetaData(_nhConfiguration)); }
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
	}
}