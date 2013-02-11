using System;
using NHibernate.Envers.Configuration;

namespace NHibernate.Envers.Query.Order
{
	public interface IAuditOrder
	{
		/// <summary>
		/// </summary>
		/// <param name="auditCfg">Configuration.</param>
		/// <returns>A pair: (property name, ascending?).</returns>
		Tuple<string, bool> GetData(AuditConfiguration auditCfg);
	}
}
