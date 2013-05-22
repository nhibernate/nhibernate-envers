using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Query.Impl
{
	public class EntitiesAtRevisionQuery : AbstractAuditQuery
	{
		private readonly long _revision;
		private readonly bool _includeDeletions;

		public EntitiesAtRevisionQuery(AuditConfiguration verCfg,
										IAuditReaderImplementor versionsReader,
										System.Type cls,
										long revision,
										bool includeDeletions)
			: base(verCfg, versionsReader, cls)
		{
			_revision = revision;
			_includeDeletions = includeDeletions;
		}

		public EntitiesAtRevisionQuery(AuditConfiguration verCfg,
										IAuditReaderImplementor versionsReader,
										string entityName,
										long revision,
										bool includeDeletions)
			: base(verCfg, versionsReader, entityName)
		{
			_revision = revision;
			_includeDeletions = includeDeletions;
		}

		protected override void FillResult(IList result)
		{
			/*
			 * The query that we need to create:
			 *   SELECT new list(e) FROM versionsReferencedEntity e
			 *   WHERE
			 * (all specified conditions, transformed, on the "e" entity) AND
			 * (selecting e entities at revision :revision)
			 *   --> for DefaultAuditStrategy:
			 *     e.revision = (SELECT max(e2.revision) FROM versionsReferencedEntity e2
			 *       WHERE e2.revision <= :revision AND e2.id = e.id) 
			 *     
			 *   --> for ValidityAuditStrategy:
			 *     e.revision <= :revision and (e.endRevision > :revision or e.endRevision is null)
			 *     
			 *     AND
			 * (only non-deleted entities)
			 *     e.revision_type != DEL
			 */
			var verEntCfg = VerCfg.AuditEntCfg;
			var revisionPropertyPath = verEntCfg.RevisionNumberPath;
			var originalIdPropertyName = verEntCfg.OriginalIdPropName;

			var referencedIdData = new MiddleIdData(verEntCfg, VerCfg.EntCfg[EntityName].IdMappingData,
					null, EntityName, VerCfg.EntCfg.IsVersioned(EntityName));

			// (selecting e entities at revision :revision)
			// --> based on auditStrategy (see above)
			VerCfg.GlobalCfg.AuditStrategy.AddEntityAtRevisionRestriction(QueryBuilder, QueryBuilder.RootParameters, revisionPropertyPath,
					verEntCfg.RevisionEndFieldName, true, referencedIdData,
					revisionPropertyPath, originalIdPropertyName, QueryConstants.ReferencedEntityAlias, QueryConstants.ReferencedEntityAliasDefAudStr);

			// e.revision_type != DEL
			if (!_includeDeletions)
			{
				QueryBuilder.RootParameters.AddWhereWithParam(verEntCfg.RevisionTypePropName, "<>", RevisionType.Deleted);				
			}

			// all specified conditions
			foreach (var criterion in Criterions)
			{
				criterion.AddToQuery(VerCfg, VersionsReader, EntityName, QueryBuilder, QueryBuilder.RootParameters);
			}


			var query = BuildQuery();
			// add named parameter (only used for ValidAuditTimeStrategy) 
			if (query.NamedParameters.Contains(QueryConstants.RevisionParameter))
			{
				query.SetParameter(QueryConstants.RevisionParameter, _revision);
			}

			if (HasProjection)
			{
				query.List(result);
				return;
			}
			var queryResult = new List<IDictionary>();
			query.List(queryResult);
			EntityInstantiator.AddInstancesFromVersionsEntities(EntityName, result, queryResult, _revision);
		}
	}
}
