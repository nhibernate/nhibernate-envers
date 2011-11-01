using System;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Query.Order
{
	public interface IAuditOrder
	{
		/// <summary>
		/// </summary>
		/// <param name="auditCfg">Configuration.</param>
		/// <returns>A pair: (property name, ascending?).</returns>
		Pair<String, Boolean> GetData(AuditConfiguration auditCfg);
	}
}
