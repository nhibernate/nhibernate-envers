using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using log4net;
using NHibernate.Envers.Configuration.Store;

namespace NHibernate.Envers.Configuration.Fluent
{
	public class FluentConfiguration : IMetaDataProvider
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (FluentConfiguration));
		private readonly IList<IAttributesPerMethodInfoFactory> attributeFactories;
		private readonly ICollection<System.Type> auditedTypes;

		public FluentConfiguration()
		{
			attributeFactories = new List<IAttributesPerMethodInfoFactory>();
			auditedTypes = new List<System.Type>();
		}

		public IFluentAudit<T> Audit<T>()
		{
			var ret = new FluentAudit<T>();
			attributeFactories.Add(ret);
			auditedTypes.Add(typeof(T));
			return ret;
		}

		public void SetRevisionEntity<T>(Expression<Func<T, object>> revisionNumber, Expression<Func<T, object>> revisionTimestamp)
		{
			attributeFactories.Add(new FluentRevision(typeof (T), 
								revisionNumber.Body.MethodInfo("revisionNumber"),
								revisionTimestamp.Body.MethodInfo("revisionTimestamp")));
		}

		public IDictionary<System.Type, IEntityMeta> CreateMetaData(Cfg.Configuration nhConfiguration)
		{
			var ret = new Dictionary<System.Type, IEntityMeta>();
			foreach (var attributeFactory in attributeFactories)
			{
				var factoryResult = attributeFactory.Create();
				foreach (var memberInfoAndAttributes in factoryResult)
				{
					var memberInfo = memberInfoAndAttributes.Key;
					var classType = memberInfo as System.Type;
					foreach (var attribute in memberInfoAndAttributes.Value)
					{
						var attrType = attribute.GetType();
						if (classType != null)
						{
							var entMeta = createOrGetEntityMeta(ret, classType);
							log.Debug("Adding " + attrType.Name + " to type " + classType.FullName);
							entMeta.AddClassMeta(attribute);
						}
						else
						{
							var memberType = memberInfo.DeclaringType;
							var entMeta = createOrGetEntityMeta(ret, memberType);
							log.Debug("Adding " + attrType.Name + " to type " + memberType.FullName);
							entMeta.AddMemberMeta(memberInfo, attribute);	
						}
					}
				}
			}
			addBaseTypesForAuditAttribute(ret, auditedTypes);
			return ret;
		}

		private static void addBaseTypesForAuditAttribute(IDictionary<System.Type, IEntityMeta> ret, IEnumerable<System.Type> auditedTypes)
		{
			foreach (var auditedType in auditedTypes)
			{
				setBaseTypeAsAudited(auditedType.BaseType, ret);
			}
		}

		private static void setBaseTypeAsAudited(System.Type baseType, IDictionary<System.Type, IEntityMeta> ret)
		{
			if (!ret.ContainsKey(baseType) && !baseType.Equals(typeof(object)))
			{
				var eMeta = new EntityMeta();
				eMeta.AddClassMeta(new AuditedAttribute());
				ret[baseType] = eMeta;

				setBaseTypeAsAudited(baseType.BaseType, ret);
			}
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