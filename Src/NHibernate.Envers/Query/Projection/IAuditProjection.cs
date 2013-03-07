using System;
using NHibernate.Envers.Configuration;

namespace NHibernate.Envers.Query.Projection
{
	public interface IAuditProjection
	{
		/// <summary>
		/// </summary>
		/// <param name="auditCfg">Configuration.</param>
		/// <returns>A triple: (function name - possibly null, property name, add distinct?).</returns>
		Tuple<string, string, Boolean> GetData(AuditConfiguration auditCfg);
	}
}
