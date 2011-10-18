﻿using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Query.Impl
{
	/// <summary>
	/// In comparison to <see cref="EntitiesAtRevisionQuery"/> this query will return an empty collection if an entity
	/// of a certain type has not been changed in a given revision.
	/// </summary>
	public class EntitiesModifiedAtRevisionQuery : AbstractAuditQuery
	{
		private readonly long _revision;

		public EntitiesModifiedAtRevisionQuery(AuditConfiguration verCfg, IAuditReaderImplementor versionsReader,
															System.Type cls, long revision)
			: base(verCfg, versionsReader, cls)
		{
			_revision = revision;
		}

		protected override void FillResult(IList result)
		{
			/*
		  * The query that we need to create:
		  *   SELECT new list(e) FROM versionsReferencedEntity e
		  *   WHERE
		  * (all specified conditions, transformed, on the "e" entity) AND
		  * e.revision = :revision
		  */

			var verEntCfg = VerCfg.AuditEntCfg;
			var revisionPropertyPath = verEntCfg.RevisionNumberPath;
			QueryBuilder.RootParameters.AddWhereWithParam(revisionPropertyPath, "=", _revision);

			foreach (var criterion in Criterions)
			{
				criterion.AddToQuery(VerCfg, EntityName, QueryBuilder, QueryBuilder.RootParameters);
			}

			var query = BuildQuery();


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