using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Configuration.Metadata;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Envers.Entities.Mapper.Relation;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Synchronization;
using NHibernate.Envers.Tools.Query;
using NHibernate.Type;

namespace NHibernate.Envers.Strategy
{
	/// <summary>
	/// Audit strategy which persists and retrieves audit information using a validity algorithm, based on the 
	/// start-revision and end-revision of a row in the audit tables. 
	/// <p>This algorithm works as follows:
	/// <ul>
	/// <li>For a <strong>new row</strong> that is persisted in an audit table, only the <strong>start-revision</strong> column of that row is set</li>
	/// <li>At the same time the <strong>end-revision</strong> field of the <strong>previous</strong> audit row is set to this revision</li>
	/// <li>Queries are retrieved using 'between start and end revision', instead of a subquery.</li>
	/// </ul>
	/// </p>
	/// 
	/// <p>
	/// This has a few important consequences that need to be judged against against each other:
	/// <ul>
	/// <li>Persisting audit information is a bit slower, because an extra row is updated</li>
	/// <li>Retrieving audit information is a lot faster</li>
	/// </ul>
	/// </p>
	/// </summary>
	[Serializable]
	public partial class ValidityAuditStrategy : IAuditStrategy
	{
		private AuditConfiguration _auditConfiguration;

		public void Initialize(AuditConfiguration auditConfiguration)
		{
			_auditConfiguration = auditConfiguration;
		}

		public void Perform(ISession session, string entityName, object id, object data, object revision)
		{
			var auditedEntityName = _auditConfiguration.AuditEntCfg.GetAuditEntityName(entityName);

			var reuseEntityIdentifier = _auditConfiguration.GlobalCfg.AllowIdentifierReuse;
			// Update the end date of the previous row if this operation is expected to have a previous row
			var revisionTypeIsAdded = revisionType(_auditConfiguration, data) == RevisionType.Added;
			if (reuseEntityIdentifier || !revisionTypeIsAdded)
			{
				/*
				 Constructing a query:
				 select e from audited_ent e where e.end_rev is null and e.id = :id
				*/
				var qb = new QueryBuilder(auditedEntityName, QueryConstants.MiddleEntityAlias);

				// e.id = :id
				var idMapper = _auditConfiguration.EntCfg[entityName].IdMapper;
				idMapper.AddIdEqualsToQuery(qb.RootParameters, id, _auditConfiguration.AuditEntCfg.OriginalIdPropName, true);

				addEndRevisionNullRestriction(_auditConfiguration, qb);

				var l = qb.ToQuery(session).SetLockMode(QueryConstants.MiddleEntityAlias, LockMode.Upgrade).List();

				updateLastRevision(session, _auditConfiguration, l, id, auditedEntityName, revision, (!reuseEntityIdentifier || !revisionTypeIsAdded));
			}

			// Save the audit data
			session.Save(auditedEntityName, data);
			SessionCacheCleaner.ScheduleAuditDataRemoval(session, data);
		}

		public void PerformCollectionChange(ISession session, string entityName, string propertyName, AuditConfiguration auditCfg, PersistentCollectionChangeData persistentCollectionChangeData, object revision)
		{
			var qb = new QueryBuilder(persistentCollectionChangeData.EntityName, QueryConstants.MiddleEntityAlias);

			var originalIdPropName = _auditConfiguration.AuditEntCfg.OriginalIdPropName;
			var originalId = (IDictionary)persistentCollectionChangeData.Data[originalIdPropName];
			var revisionFieldName = auditCfg.AuditEntCfg.RevisionFieldName;
			var revisionTypePropName = auditCfg.AuditEntCfg.RevisionTypePropName;

			// Adding a parameter for each id component, except the rev number and type.
			foreach (DictionaryEntry originalIdKeyValue in originalId)
			{
				if (!revisionFieldName.Equals(originalIdKeyValue.Key) && !revisionTypePropName.Equals(originalIdKeyValue.Key))
				{
					qb.RootParameters.AddWhereWithParam(originalIdPropName + "." + originalIdKeyValue.Key, true, "=", originalIdKeyValue.Value);
				}
			}

			var sessionFactory = ((ISessionImplementor)session).Factory;
			var propertyType = sessionFactory.GetEntityPersister(entityName).GetPropertyType(propertyName);
			if (propertyType.IsCollectionType)
			{
				var collectionPropertyType = (CollectionType)propertyType;
				// Handling collection of components.
				if (collectionPropertyType.GetElementType(sessionFactory) is ComponentType)
				{
					// Adding restrictions to compare data outside of primary key.
					// todo: is it necessary that non-primary key attributes be compared?
					foreach (var dataEntry in persistentCollectionChangeData.Data)
					{
						if (!originalIdPropName.Equals(dataEntry.Key))
						{
							if (dataEntry.Value != null)
							{
								qb.RootParameters.AddWhereWithParam(dataEntry.Key, true, "=", dataEntry.Value);
							}
							else
							{
								qb.RootParameters.AddNullRestriction(dataEntry.Key, true);
							}
						}
					}
				}
			}

			addEndRevisionNullRestriction(_auditConfiguration, qb);

			var l = qb.ToQuery(session).SetLockMode(QueryConstants.MiddleEntityAlias, LockMode.Upgrade).List();

			if (l.Count > 0)
			{
				updateLastRevision(session, _auditConfiguration, l, originalId, persistentCollectionChangeData.EntityName, revision, true);
			}

			// Save the audit data
			var data = persistentCollectionChangeData.Data;
			session.Save(persistentCollectionChangeData.EntityName, data);
			SessionCacheCleaner.ScheduleAuditDataRemoval(session, data);
		}

		private static void addEndRevisionNullRestriction(AuditConfiguration auditCfg, QueryBuilder qb)
		{
			// e.end_rev is null
			qb.RootParameters.AddWhere(auditCfg.AuditEntCfg.RevisionEndFieldName, true, "is", "null", false);
		}

		public void AddEntityAtRevisionRestriction(QueryBuilder rootQueryBuilder, Parameters parameters, string revisionProperty, string revisionEndProperty, bool addAlias, MiddleIdData idData, string revisionPropertyPath, string originalIdPropertyName, string alias1, string alias2)
		{
			addRevisionRestriction(parameters, revisionProperty, revisionEndProperty, addAlias, true);
		}

		public void AddAssociationAtRevisionRestriction(QueryBuilder rootQueryBuilder, Parameters parameters, string revisionProperty, string revisionEndProperty, bool addAlias, MiddleIdData referencingIdData, string versionsMiddleEntityName, string eeOriginalIdPropertyPath, string revisionPropertyPath, string originalIdPropertyName, string alias1, bool inclusive, params MiddleComponentData[] componentDatas)
		{
			addRevisionRestriction(parameters, revisionProperty, revisionEndProperty, addAlias, inclusive);
		}

		/// <summary>
		/// Adds a <![CDATA[<many-to-one>]]> mapping to the revision entity as an endrevision.
		/// Also, if <see cref="AuditEntitiesConfiguration.IsRevisionEndTimestampEnabled"/> set, adds a timestamp when the revision is no longer valid.
		/// </summary>
		public void AddExtraRevisionMapping(XElement classMapping, XElement revisionInfoRelationMapping)
		{
			var verEntCfg = _auditConfiguration.AuditEntCfg;
			var manyToOne = MetadataTools.AddManyToOne(classMapping, verEntCfg.RevisionEndFieldName, verEntCfg.RevisionInfoEntityAssemblyQualifiedName, true, true);
			manyToOne.Add(revisionInfoRelationMapping.Elements());
			MetadataTools.AddOrModifyColumn(manyToOne, verEntCfg.RevisionEndFieldName);

			if (verEntCfg.IsRevisionEndTimestampEnabled)
			{
				const string revisionInfoTimestampSqlType = "Timestamp";
				MetadataTools.AddProperty(classMapping, verEntCfg.RevisionEndTimestampFieldName, revisionInfoTimestampSqlType, true, true, false, null);
			}
		}

		private static void addRevisionRestriction(Parameters rootParameters, string revisionProperty, string revisionEndProperty, bool addAlias, bool inclusive)
		{
			// e.revision <= _revision and (e.endRevision > _revision or e.endRevision is null)
			var subParm = rootParameters.AddSubParameters("or");
			rootParameters.AddWhereWithNamedParam(revisionProperty, addAlias, inclusive ? "<=" : "<", QueryConstants.RevisionParameter);
			subParm.AddWhereWithNamedParam(revisionEndProperty + ".id", addAlias, inclusive ? ">" : ">=", QueryConstants.RevisionParameter);
			subParm.AddWhere(revisionEndProperty, addAlias, "is", "null", false);
		}

		private static RevisionType revisionType(AuditConfiguration auditCfg, object data)
		{
			return (RevisionType)((IDictionary<string, object>)data)[auditCfg.AuditEntCfg.RevisionTypePropName];
		}

		private void updateLastRevision(ISession session, AuditConfiguration auditCfg, IList l,
									object id, string auditedEntityName, object revision, bool throwIfNotOneEntry)
		{
			// There should be one entry
			if (l.Count == 1)
			{
				// Setting the end revision to be the current rev
				var previousData = (IDictionary) l[0];
				var revisionEndFieldName = auditCfg.AuditEntCfg.RevisionEndFieldName;
				previousData[revisionEndFieldName] = revision;

				if (auditCfg.AuditEntCfg.IsRevisionEndTimestampEnabled)
				{
					// Determine the value of the revision property annotated with @RevisionTimestamp
					DateTime revisionEndTimestamp;
					var revEndTimestampFieldName = auditCfg.AuditEntCfg.RevisionEndTimestampFieldName;
					var revEndTimestampObj = _auditConfiguration.RevisionTimestampGetter.Get(revision);

					// convert to a DateTime
					if (revEndTimestampObj is DateTime)
					{
						revisionEndTimestamp = (DateTime) revEndTimestampObj;
					}
					else
					{
						revisionEndTimestamp = new DateTime((long) revEndTimestampObj);
					}

					// Setting the end revision timestamp
					previousData[revEndTimestampFieldName] = revisionEndTimestamp;
				}

				// Saving the previous version
				session.Save(auditedEntityName, previousData);
				SessionCacheCleaner.ScheduleAuditDataRemoval(session, previousData);
			}
			else
			{
				if (throwIfNotOneEntry)
				{
					throw new InvalidOperationException("Cannot find previous revision for entity " + auditedEntityName + " and id " + id);					
				}
			}
		}
	}
}