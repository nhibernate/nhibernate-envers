using System;
using System.Collections.Generic;
using System.Text;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools.Query;
using NHibernate.Transform;

namespace NHibernate.Envers.Entities.Mapper.Relation.Query
{
	[Serializable]
	public abstract class AbstractRelationQueryGenerator : IRelationQueryGenerator
	{
		private readonly bool _revisionTypeInId;

		protected AbstractRelationQueryGenerator(AuditEntitiesConfiguration verEntCfg, 
																						MiddleIdData referencingIdData, 
																						bool revisionTypeInId)
		{
			VerEntCfg = verEntCfg;
			ReferencingIdData = referencingIdData;
			_revisionTypeInId = revisionTypeInId;
		}

		protected MiddleIdData ReferencingIdData { get; private set; }
		protected AuditEntitiesConfiguration VerEntCfg { get; private set; }

		/// <summary>
		/// Query used to retrieve state of audited entity valid at a given revision.
		/// </summary>
		protected abstract string QueryString();

		/// <summary>
		/// Query executed to retrieve state of audited entity valid at previous revision
		/// or removed during exactly specified revision number. Used only when traversing deleted
		/// entities graph.
		/// </summary>
		protected abstract string QueryRemovedString();

		protected virtual bool TransformResultToList()
		{
			return false;
		}

		public IQuery GetQuery(IAuditReaderImplementor versionsReader, object primaryKey, long revision, bool removed)
		{
			var query = versionsReader.Session.CreateQuery(removed ? QueryRemovedString() : QueryString())
			                          .SetParameter(QueryConstants.RevisionParameter, revision)
			                          .SetParameter(QueryConstants.DelRevisionTypeParameter, RevisionType.Deleted);

			foreach (var paramData in ReferencingIdData.PrefixedMapper.MapToQueryParametersFromId(primaryKey))
			{
				paramData.SetParameterValue(query);
			}

			if(TransformResultToList())
			{
				query.SetResultTransformer(Transformers.ToList);
			}
			return query;
		}

		protected string RevisionTypePath()
		{
			return _revisionTypeInId
				       ? VerEntCfg.OriginalIdPropName + "." + VerEntCfg.RevisionTypePropName
				       : VerEntCfg.RevisionTypePropName;
		}

		protected string QueryToString(QueryBuilder query)
		{
			return QueryToString(query, new Dictionary<string, object>());
		}

		protected string QueryToString(QueryBuilder query, IDictionary<string, object> queryParamValues)
		{
			var sb = new StringBuilder();
			query.Build(sb, queryParamValues);
			return sb.ToString();
		}
	}
}