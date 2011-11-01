using System;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Query.Projection
{
	public interface IAuditProjection
	{
		/// <summary>
		/// </summary>
		/// <param name="auditCfg">Configuration.</param>
		/// <returns>A triple: (function name - possibly null, property name, add distinct?).</returns>
		Triple<String, String, Boolean> GetData(AuditConfiguration auditCfg);
	}
}
