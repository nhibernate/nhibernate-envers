﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Synchronization;
using NHibernate.Envers.Tools.Reflection;
using NHibernate.Properties;

namespace NHibernate.Envers.RevisionInfo
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class DefaultRevisionInfoGenerator : IRevisionInfoGenerator
	{

		public async Task SaveRevisionDataAsync(ISession session, object revisionData, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			await (session.SaveAsync(_revisionInfoEntityName, revisionData, cancellationToken)).ConfigureAwait(false);
			SessionCacheCleaner.ScheduleAuditDataRemoval(session, revisionData);
		}
	}
}
