﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Query.Impl
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class EntitiesModifiedAtRevisionQuery : AbstractAuditQuery
	{

		protected override async Task FillResultAsync(IList result, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
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
				criterion.AddToQuery(VerCfg, VersionsReader, EntityName, QueryBuilder, QueryBuilder.RootParameters);
			}
			foreach (var associationQuery in AssociationQueries)
			{
				associationQuery.AddCriterionsToQuery(VersionsReader);
			}

			var query = BuildQuery();
			await (ApplyProjectionsAsync(query, result, _revision, cancellationToken)).ConfigureAwait(false);
		}
	}
}