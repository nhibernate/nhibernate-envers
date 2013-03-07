using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Query.Criteria
{
	public class AuditDisjunction : IExtendableCriterion
	{
		private readonly IList<IAuditCriterion> criterions;

		public AuditDisjunction()
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
			var orParameters = parameters.AddSubParameters(Parameters.OR);

			if (criterions.Count == 0)
			{
				orParameters.AddWhere("0", false, "=", "1", false);
			}
			else
			{
				foreach (var criterion in criterions)
				{
					criterion.AddToQuery(verCfg, versionsReader, entityName, qb, orParameters);
				}
			}
		}
	}
}
