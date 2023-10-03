﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Entities.Mapper.Id;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	using System.Threading.Tasks;
	using System.Threading;
	public partial class ToOneIdMapper : AbstractToOneMapper 
	{

		protected override async Task NullSafeMapToEntityFromMapAsync(AuditConfiguration verCfg, object obj, IDictionary data, object primaryKey, IAuditReaderImplementor versionsReader, long revision, CancellationToken cancellationToken = default(CancellationToken))
		{
			cancellationToken.ThrowIfCancellationRequested();
			var entityId = _delegat.MapToIdFromMap(data);
			object value = null;
			if (entityId != null)
			{
				if (!versionsReader.FirstLevelCache.TryGetValue(_referencedEntityName, revision, entityId, out value))
				{
					var referencedEntity = GetEntityInfo(verCfg, _referencedEntityName);
					var ignoreNotFound = false;
					if (!referencedEntity.IsAudited)
					{
						var referencingEntityName = verCfg.EntCfg.GetEntityNameForVersionsEntityName((string) data["$type$"]);
						var relation = verCfg.EntCfg.GetRelationDescription(referencingEntityName, PropertyData.Name);
						ignoreNotFound = relation != null && relation.IsIgnoreNotFound;
					}
					var removed = RevisionType.Deleted.Equals(data[verCfg.AuditEntCfg.RevisionTypePropName]);

					value = ignoreNotFound ? 
						await (ToOneEntityLoader.LoadImmediateAsync(versionsReader, _referencedEntityName, entityId, revision, removed, verCfg, cancellationToken)).ConfigureAwait(false) : 
						await (ToOneEntityLoader.CreateProxyOrLoadImmediateAsync(versionsReader, _referencedEntityName, entityId, revision, removed, verCfg, cancellationToken)).ConfigureAwait(false);
				}
			}
			SetPropertyValue(obj, value);
		}
	}
}
