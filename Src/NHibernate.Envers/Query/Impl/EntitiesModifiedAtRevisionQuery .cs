using System.Collections;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Query.Impl
{
	/// <summary>
	/// In comparison to <see cref="EntitiesAtRevisionQuery"/> this query returns an empty collection if an entity
	/// of a certain type has not been changed in a given revision.
	/// </summary>
	public partial class EntitiesModifiedAtRevisionQuery : AbstractAuditQuery
	{
		private readonly long _revision;

		public EntitiesModifiedAtRevisionQuery(AuditConfiguration verCfg, IAuditReaderImplementor versionsReader,
															string entityName, long revision)
			: base(verCfg, versionsReader, entityName)
		{
			_revision = revision;
		}

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
				criterion.AddToQuery(VerCfg, VersionsReader, EntityName, QueryBuilder, QueryBuilder.RootParameters);
			}
			foreach (var associationQuery in AssociationQueries)
			{
				associationQuery.AddCriterionsToQuery(VersionsReader);
			}

			var query = BuildQuery();
			ApplyProjections(query, result, _revision);
		}
	}
}