﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Query.Criteria;
using NHibernate.Envers.Query.Order;
using NHibernate.Envers.Query.Projection;
using NHibernate.SqlCommand;

namespace NHibernate.Envers.Query
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial interface IAuditQuery 
	{
		Task<IList> GetResultListAsync(CancellationToken cancellationToken = default(CancellationToken));
		Task<IList<T>> GetResultListAsync<T>(CancellationToken cancellationToken = default(CancellationToken));

		Task<object> GetSingleResultAsync(CancellationToken cancellationToken = default(CancellationToken));

		Task<T> GetSingleResultAsync<T>(CancellationToken cancellationToken = default(CancellationToken));
	}
}
