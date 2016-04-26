using System;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities;

namespace NHibernate.Envers.Query.Projection
{
	public interface IAuditProjection
	{
		/// <summary>
		/// </summary>
		/// <param name="auditCfg">Configuration.</param>
		/// <returns>A triple: (function name - possibly null, property name, add distinct?).</returns>
		Tuple<string, string, bool> GetData(AuditConfiguration auditCfg);

		object ConvertQueryResult(AuditConfiguration auditCfg, EntityInstantiator entityInstantiator, string entityName, long revision, object value);
	}
}
