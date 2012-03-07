using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Configuration.Store;

namespace NHibernate.Envers.Configuration.Fluent
{
	/// <summary>
	/// This implementation of <see cref="IMetaDataProvider"/>
	/// is used to programmaticly configure envers.
	/// </summary>
	public class FluentConfiguration : IMetaDataProvider
	{
		private static readonly IInternalLogger log = LoggerProvider.LoggerFor(typeof(FluentConfiguration));
		private readonly IList<IAttributeProvider> attributeFactories;

		public FluentConfiguration()
		{
			attributeFactories = new List<IAttributeProvider>();
		}

		/// <summary>
		/// Register entity type T to be audited.
		/// </summary>
		/// <typeparam name="T">The type to be audited.</typeparam>
		/// <returns>A fluent audit object where you can fine grain the auditing.</returns>
		public IFluentAudit<T> Audit<T>()
		{
			var ret = new FluentAudit<T>();
			attributeFactories.Add(ret);
			return ret;
		}

		/// <summary>
		/// Register an entity type to be audited.
		/// </summary>
		/// <param name="type">The type to be audited.</param>
		/// <returns>A fluent audit object where you can fine grain the auditing.</returns>
		public IFluentAudit Audit(System.Type type)
		{
			var ret = new LooselyTypedFluentAudit(type);
			attributeFactories.Add(ret);
			return ret;
		}
		
		/// <summary>
		/// Register multiple audited entities.
		/// </summary>
		/// <param name="types">All types to be audited.</param>
		/// <remarks>
		/// Each class will be audited using default values of <see cref="AuditedAttribute"/> without exclusions of properties.
		/// </remarks>
		public void Audit(IEnumerable<System.Type> types)
		{
			foreach (var type in types)
			{
				Audit(type);
			}
		}

		/// <summary>
		/// Defines a custom revision entity.
		/// </summary>
		/// <typeparam name="T">The custom revision entity type</typeparam>
		/// <param name="revisionNumber">Revision number property on custom revision entity</param>
		/// <param name="revisionTimestamp">Revision timestamp property on custom revision entity</param>
		public void SetRevisionEntity<T>(Expression<Func<T, object>> revisionNumber,
													 Expression<Func<T, object>> revisionTimestamp)
		{
			setRevisionEntity(revisionNumber, revisionTimestamp, null, null);
		}

		/// <summary>
		/// Defines a custom revision entity with a custom <see cref="IRevisionListener"/>.
		/// </summary>
		/// <typeparam name="T">The custom revision entity type</typeparam>
		/// <param name="revisionNumber">Revision number property on custom revision entity</param>
		/// <param name="revisionTimestamp">Revision timestamp property on custom revision entity</param>
		/// <param name="revisionListener">The listener singleton</param>
		public void SetRevisionEntity<T>(Expression<Func<T, object>> revisionNumber,
													Expression<Func<T, object>> revisionTimestamp,
													IRevisionListener revisionListener)
		{
			setRevisionEntity(revisionNumber, revisionTimestamp, null, revisionListener);
		}

		/// <summary>
		/// Defines a custom revision entity.
		/// </summary>
		/// <typeparam name="T">The custom revision entity type</typeparam>
		/// <param name="revisionNumber">Revision number property on custom revision entity</param>
		/// <param name="revisionTimestamp">Revision timestamp property on custom revision entity</param>
		/// <param name="modifiedEntityNames">
		/// Modified entity names property on custom revision entity.
		/// Must be of type <code>ISet{string}</code>.
		/// </param>
		public void SetRevisionEntity<T>(Expression<Func<T, object>> revisionNumber,
													Expression<Func<T, object>> revisionTimestamp,
													Expression<Func<T, object>> modifiedEntityNames)
		{
			setRevisionEntity(revisionNumber, revisionTimestamp, modifiedEntityNames, null);
		}

		private void setRevisionEntity<T>(Expression<Func<T, object>> revisionNumber,
													Expression<Func<T, object>> revisionTimestamp,
													Expression<Func<T, object>> modifiedEntityNames,
													IRevisionListener revisionListener)
		{
			var modEntNames = modifiedEntityNames == null ? 
									null : 
									modifiedEntityNames.MethodInfo();
			attributeFactories.Add(new FluentRevision(typeof(T),
								  revisionNumber.MethodInfo(),
								  revisionTimestamp.MethodInfo(),
								  modEntNames,
								  revisionListener));
		}

		public IDictionary<System.Type, IEntityMeta> CreateMetaData(Cfg.Configuration nhConfiguration)
		{
			var ret = new Dictionary<System.Type, IEntityMeta>();
			var auditedTypes = new HashSet<System.Type>();
			foreach (var attributeFactory in attributeFactories)
			{
				foreach (var memberInfoAndAttribute in attributeFactory.Attributes(nhConfiguration))
				{
					var type = memberInfoAndAttribute.Type;
					var entMeta = createOrGetEntityMeta(ret, type);
					if (memberInfoAndAttribute.MemberInfo.MemberType == MemberTypes.TypeInfo)
						addClassMetaAndLog(type, memberInfoAndAttribute.Attribute, entMeta);
					else
						addMemberMetaAndLog(type, memberInfoAndAttribute, entMeta);
					if (memberInfoAndAttribute.Attribute.GetType() == typeof(AuditedAttribute))
						auditedTypes.Add(type);
				}
			}
			addBaseTypesForAuditAttribute(ret, auditedTypes);
			throwIfUsingNonMappedRevisionEntity(nhConfiguration);
			return ret;
		}

		private void throwIfUsingNonMappedRevisionEntity(Cfg.Configuration nhConfiguration)
		{
			foreach (var revAttr in attributeFactories.OfType<FluentRevision>()
										.Where(revAttr => nhConfiguration.GetClassMapping(revAttr.RevisionEntityType) == null))
			{
				throw new MappingException("Custom revision entity " + revAttr.RevisionEntityType + " must be mapped!");
			}
		}

		private static void addMemberMetaAndLog(System.Type type, MemberInfoAndAttribute memberInfoAndAttribute, EntityMeta entMeta)
		{
			log.DebugFormat("Adding {0} to member {1} on type {2}.",
							memberInfoAndAttribute.Attribute.GetType().Name,
							memberInfoAndAttribute.MemberInfo.Name,
							type.FullName);
			entMeta.AddMemberMeta(memberInfoAndAttribute.MemberInfo, memberInfoAndAttribute.Attribute);
		}

		private static void addClassMetaAndLog(System.Type type, Attribute classAttribute, EntityMeta entMeta)
		{
			log.DebugFormat("Adding {0} to type {1}.", classAttribute.GetType().Name, type.FullName);
			entMeta.AddClassMeta(classAttribute);
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
			if (baseType == typeof(object))
				return;

			IEntityMeta entMetaForBaseTypeTemp;
			if (!ret.TryGetValue(baseType, out entMetaForBaseTypeTemp))
			{
				entMetaForBaseTypeTemp = new EntityMeta();
			}
			var entMetaForBaseType = (EntityMeta)entMetaForBaseTypeTemp;
			if (!entityMetaIsAuditedClass(entMetaForBaseType))
			{
				addClassMetaAndLog(baseType, new AuditedAttribute(), entMetaForBaseType);
				ret[baseType] = entMetaForBaseType;
			}
			setBaseTypeAsAudited(baseType.BaseType, ret);
		}

		private static bool entityMetaIsAuditedClass(IEntityMeta entMetaForBaseType)
		{
			return entMetaForBaseType.ClassMetas.Any(classMeta => classMeta.GetType() == typeof(AuditedAttribute));
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