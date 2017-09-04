using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NHibernate.Envers.Configuration.Store
{
	public class MetaDataStore : IMetaDataStore
	{
		private readonly Cfg.Configuration _nhConfiguration;

		public MetaDataStore(Cfg.Configuration nhConfiguration, 
							IMetaDataProvider metaDataProvider,
							IEnumerable<IMetaDataAdder> metaDataAdders)
		{
			_nhConfiguration = nhConfiguration;
			EntityMetas = initializeMetas(metaDataProvider, metaDataAdders);
		}

		public IDictionary<System.Type, IEntityMeta> EntityMetas { get; }

		private IDictionary<System.Type, IEntityMeta> initializeMetas(IMetaDataProvider metaDataProvider,
																				IEnumerable<IMetaDataAdder> metaDataAdders)
		{
			var metaData = metaDataProvider.CreateMetaData(_nhConfiguration);
			foreach (var metaDataAdder in metaDataAdders)
			{
				metaDataAdder.AddMetaDataTo(metaData);
			}
			return metaData;
		}

		public T ClassMeta<T>(System.Type entityType) where T : Attribute
		{
			var attrType = typeof (T);
			if (!EntityMetas.TryGetValue(entityType, out var entityMeta))
				return null;
			return entityMeta.ClassMetas.Where(enversAttribute => enversAttribute.GetType() == attrType).Cast<T>().FirstOrDefault();
		}

		public T MemberMeta<T>(MemberInfo member) where T : Attribute
		{
			var attrType = typeof(T);
			if (!EntityMetas.TryGetValue(member.ReflectedType, out var entityMeta))
				return null;
			if (!entityMeta.MemberMetas.TryGetValue(member, out var attributes))
				return null;

			foreach (var enversAttribute in attributes)
			{
				if (enversAttribute.GetType() == attrType)
					return (T)enversAttribute;
			}
			return null;
		}
	}
}