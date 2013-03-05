using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query.Criteria
{
	public class AuditConjunction : IExtendableCriterion
	{
		private readonly IList<IAuditCriterion> criterions;

		public AuditConjunction()
		{
			criterions = new List<IAuditCriterion>();
		}

		public IExtendableCriterion Add(IAuditCriterion criterion)
		{
			criterions.Add(criterion);
			return this;
		}

		public void AddToQuery(AuditConfiguration verCfg, IAuditReaderImplementor versionsReader, string entityName, QueryBuilder qb, Parameters parameters)
		{
			var andParameters = parameters.AddSubParameters(Parameters.AND);

			if (criterions.Count == 0)
			{
				andParameters.AddWhere("1", false, "=", "1", false);
			}
			else
			{
				foreach (var criterion in criterions)
				{
					criterion.AddToQuery(verCfg, versionsReader, entityName, qb, andParameters);
				}
			}
		}
	}
}
