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
using System.Linq;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Relation.Query;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Query.Impl
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class HistoryQuery<TEntity, TRevisionEntity> : AbstractRevisionsQuery<IRevisionEntityInfo<TEntity, TRevisionEntity>>
		where TEntity : class
		where TRevisionEntity : class
	{

		public override Task<IEnumerable<IRevisionEntityInfo<TEntity, TRevisionEntity>>> ResultsAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<IEnumerable<IRevisionEntityInfo<TEntity, TRevisionEntity>>>(cancellationToken);
			}
			try
			{
				return Task.FromResult<IEnumerable<IRevisionEntityInfo<TEntity, TRevisionEntity>>>(Results());
			}
			catch (System.Exception ex)
			{
				return Task.FromException<IEnumerable<IRevisionEntityInfo<TEntity, TRevisionEntity>>>(ex);
			}
		}
	}
}