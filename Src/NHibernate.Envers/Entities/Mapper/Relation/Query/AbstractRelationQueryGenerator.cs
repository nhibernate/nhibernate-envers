using System;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;
using NHibernate.Transform;

namespace NHibernate.Envers.Entities.Mapper.Relation.Query
{
	[Serializable]
	public abstract class AbstractRelationQueryGenerator : IRelationQueryGenerator
	{
		private readonly AuditEntitiesConfiguration _verEntCfg;
		private readonly MiddleIdData _referencingIdData;
		private readonly bool _revisionTypeInId;

		protected AbstractRelationQueryGenerator(AuditEntitiesConfiguration verEntCfg, 
																						MiddleIdData referencingIdData, 
																						bool revisionTypeInId)
		{
			_verEntCfg = verEntCfg;
			_referencingIdData = referencingIdData;
			_revisionTypeInId = revisionTypeInId;
		}

		protected abstract string QueryString();

		protected virtual bool TransformResultToList()
		{
			return false;
		}

		public IQuery GetQuery(IAuditReaderImplementor versionsReader, object primaryKey, long revision)
		{
			var query = versionsReader.Session.CreateQuery(QueryString())
			                          .SetParameter(QueryConstants.RevisionParameter, revision)
			                          .SetParameter(QueryConstants.DelRevisionTypeParameter, RevisionType.Deleted);

			foreach (var paramData in _referencingIdData.PrefixedMapper.MapToQueryParametersFromId(primaryKey))
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
				       ? _verEntCfg.OriginalIdPropName + "." + _verEntCfg.RevisionTypePropName
				       : _verEntCfg.RevisionTypePropName;
		}
	}
}