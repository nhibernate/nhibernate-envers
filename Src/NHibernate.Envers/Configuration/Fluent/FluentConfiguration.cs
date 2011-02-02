using System.Collections.Generic;
using log4net;
using NHibernate.Envers.Configuration.Store;

namespace NHibernate.Envers.Configuration.Fluent
{
	public class FluentConfiguration : IMetaDataProvider
	{
	    private static ILog log = LogManager.GetLogger(typeof (FluentConfiguration));
	    private readonly IDictionary<System.Type, IAttributeFactory> audits;

		public FluentConfiguration()
		{
			audits = new Dictionary<System.Type, IAttributeFactory>();
		}

		public IFluentAudit<T> Audit<T>()
		{
		    var type = typeof (T);
            var ret = new FluentAudit<T>();
            audits[type] = ret;
		    return ret;
		}

		public IDictionary<System.Type, IEntityMeta> CreateMetaData(Cfg.Configuration nhConfiguration)
		{
			var ret = new Dictionary<System.Type, IEntityMeta>();
			foreach (var attributeBuilder in audits.Values)
			{
				var attrs = attributeBuilder.Create();
				foreach (var membAndAttrs in attrs)
				{
					var memberInfo = membAndAttrs.Key;
					var classType = memberInfo as System.Type;
					if(classType != null)
					{
						var entMeta = createOrGetEntityMeta(ret, classType);
						foreach (var attribute in membAndAttrs.Value)
						{
                            if(log.IsDebugEnabled)
                                log.Debug("Adding " + attribute.GetType().Name + " to type " + classType.FullName);
							entMeta.AddClassMeta(attribute);							
						}
					}
					else
					{
						var memberType = memberInfo.DeclaringType;
						var entMeta = createOrGetEntityMeta(ret, memberType);
						foreach (var attribute in membAndAttrs.Value)
						{
                            if (log.IsDebugEnabled)
                                log.Debug("Adding " + attribute.GetType().Name + " to type " + memberType.FullName);
							entMeta.AddMemberMeta(memberInfo, attribute);							
						}
					}
				}
			}
			return ret;
		}

		private static EntityMeta createOrGetEntityMeta(IDictionary<System.Type, IEntityMeta> metas, System.Type type)
		{
			IEntityMeta ret;
			if (!metas.TryGetValue(type, out ret))
			{
				ret = new EntityMeta();
				metas[type] = ret;
			}
			return (EntityMeta)ret;
		}
	}
}