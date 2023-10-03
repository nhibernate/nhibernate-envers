﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Query;
using NHibernate.Envers.Tools;
using NHibernate.Event;
using NHibernate.Proxy;

namespace NHibernate.Envers.Reader
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class AuditReader : IAuditReaderImplementor
	{

		public async Task<T> FindAsync<T>(object primaryKey, long revision, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			return (T)await (FindAsync(typeof(T), primaryKey, revision, cancellationToken)).ConfigureAwait(false);
		}

		public async Task<object> FindAsync(System.Type cls, object primaryKey, long revision, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			return await (FindAsync(cls.FullName, primaryKey, revision, cancellationToken)).ConfigureAwait(false);
		}

		public async Task<object> FindAsync(string entityName, object primaryKey, long revision, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			return await (FindAsync(entityName, primaryKey, revision, false, cancellationToken)).ConfigureAwait(false);
		}

		public async Task<object> FindAsync(string entityName, object primaryKey, long revision, bool includeDeletions, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ArgumentsTools.CheckNotNull(primaryKey, "Primary key");
			ArgumentsTools.CheckPositive(revision, "Entity revision");

			if (!verCfg.EntCfg.IsVersioned(entityName))
			{
				throw new NotAuditedException(entityName, entityName + " is not versioned!");
			}

			object result;
			if (FirstLevelCache.TryGetValue(entityName, revision, primaryKey, out result))
			{
				return result;
			}

			// The result is put into the cache by the entity instantiator called from the query
			result = await (CreateQuery().ForEntitiesAtRevision(entityName, revision, includeDeletions)
				.Add(AuditEntity.Id().Eq(primaryKey)).GetSingleResultAsync(cancellationToken)).ConfigureAwait(false);

			return result;
		}

		public async Task<IEnumerable<long>> GetRevisionsAsync(System.Type cls, object primaryKey, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			var entityName = cls.FullName;
			return await (GetRevisionsAsync(entityName, primaryKey, cancellationToken)).ConfigureAwait(false);
		}

		public async Task<IEnumerable<long>> GetRevisionsAsync(string entityName, object primaryKey, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			ArgumentsTools.CheckNotNull(primaryKey, "Primary key");

			if (!verCfg.EntCfg.IsVersioned(entityName))
			{
				throw new NotAuditedException(entityName, entityName + " is not versioned!");
			}

			var resultList = await (CreateQuery().ForRevisionsOfEntity(entityName, false, true)
				.AddProjection(AuditEntity.RevisionNumber())
				.AddOrder(AuditEntity.RevisionNumber().Asc())
				.Add(AuditEntity.Id().Eq(primaryKey))
				.GetResultListAsync(cancellationToken)).ConfigureAwait(false);
			return from object revision in resultList select Convert.ToInt64(revision);
		}

		public Task<object> GetCurrentRevisionAsync(bool persist, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!(Session is IEventSource sessionAsEventSource))
			{
				throw new NotSupportedException("The provided session is not an EventSource!");
			}
			if (cancellationToken.IsCancellationRequested)
			{
				return Task.FromCanceled<object>(cancellationToken);
			}
			return InternalGetCurrentRevisionAsync();
			async Task<object> InternalGetCurrentRevisionAsync()
			{

				// Obtaining the current audit sync
				var auditSync = verCfg.AuditProcessManager.Get(sessionAsEventSource);

				// And getting the current revision data
				return await (auditSync.CurrentRevisionDataAsync(Session, persist, cancellationToken)).ConfigureAwait(false);
			}
		}

		public async Task<T> GetCurrentRevisionAsync<T>(bool persist, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			return (T)await (GetCurrentRevisionAsync(persist, cancellationToken)).ConfigureAwait(false);
		}
	}
}
