using System;
using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Reader;
using NHibernate.Envers.Tools;

namespace NHibernate.Envers.Entities
{
	public class EntityInstantiator 
	{
		private readonly AuditConfiguration verCfg;
		private readonly IAuditReaderImplementor versionsReader;

		public EntityInstantiator(AuditConfiguration verCfg, IAuditReaderImplementor versionsReader) 
		{
			this.verCfg = verCfg;
			this.versionsReader = versionsReader;
		}

		/// <summary>
		/// Creates an entity instance based on an entry from the versions table.
		/// </summary>
		/// <param name="entityName">Name of the entity, which instances should be read.</param>
		/// <param name="versionsEntity">An entry in the versions table, from which data should be mapped.</param>
		/// <param name="revision">Revision at which this entity was read.</param>
		/// <returns>An entity instance, with versioned properties set as in the versionsEntity map, and proxies created for collections.</returns>
		public object CreateInstanceFromVersionsEntity(string entityName, IDictionary versionsEntity, long revision)
		{
			const string typeKey = "$type$";

			if (versionsEntity == null) 
			{
				return null;
			}

			if(versionsEntity.Contains(typeKey))
			{
				entityName = verCfg.EntCfg.GetEntityNameForVersionsEntityName((string)versionsEntity[typeKey]);
			}

			// First mapping the primary key
			var idMapper = verCfg.EntCfg[entityName].IdMapper;
			var originalId = (IDictionary)versionsEntity[verCfg.AuditEntCfg.OriginalIdPropName];

			var primaryKey = idMapper.MapToIdFromMap(originalId);

			object ret;
			// Checking if the entity is in cache
			if (versionsReader.FirstLevelCache.TryGetValue(entityName, revision, primaryKey, out ret)) 
			{
				return ret;
			}

			// If it is not in the cache, creating a new entity instance
			try 
			{
				var cls = Toolz.ResolveEntityClass(versionsReader.SessionImplementor, entityName);
				ret = verCfg.EntCfg[entityName].Factory(cls);
			} 
			catch (Exception e) 
			{
				throw new AuditException("Cannot create instance of type " + entityName, e);
			}

			// Putting the newly created entity instance into the first level cache, in case a one-to-one bidirectional
			// relation is present (which is eagerly loaded).
			versionsReader.FirstLevelCache.Add(entityName, revision, primaryKey, ret);

			verCfg.EntCfg[entityName].PropertyMapper.MapToEntityFromMap(verCfg, ret, versionsEntity, primaryKey, versionsReader, revision);
			idMapper.MapToEntityFromMap(ret, originalId);

			verCfg.GlobalCfg.PostInstantiationListener.PostInstantiate(ret);

			// Put entity on entityName cache after mapping it from the map representation
			versionsReader.FirstLevelCache.AddEntityName(primaryKey, revision, ret, entityName);

			return ret;
		}

		public void AddInstancesFromVersionsEntities(string entityName, IList addTo, IEnumerable<IDictionary> versionsEntities, long revision)
		{
			foreach (var versionsEntity in versionsEntities) 
			{
				addTo.Add(CreateInstanceFromVersionsEntity(entityName, versionsEntity, revision));
			}
		}

		public AuditConfiguration AuditConfiguration
		{
			get { return verCfg; }
		}

		public IAuditReaderImplementor AuditReaderImplementor
		{
			get { return versionsReader; }
		}
	}
}
