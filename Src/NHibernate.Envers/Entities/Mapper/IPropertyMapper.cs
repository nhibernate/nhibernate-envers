using System.Collections;
using System.Collections.Generic;
using NHibernate.Collection;
using NHibernate.Engine;
using NHibernate.Envers.Configuration;
using NHibernate.Envers.Reader;

namespace NHibernate.Envers.Entities.Mapper
{
	public interface IPropertyMapper
	{
		/// <summary>
		/// Maps properties to the given map, basing on differences between properties of new and old objects.
		/// </summary>
		/// <param name="session">The current session.</param>
		/// <param name="data">Data to map to.</param>
		/// <param name="newObj">New state of the entity.</param>
		/// <param name="oldObj">Old state of the entity.</param>
		/// <returns>True if there are any differences between the states represented by newObj and oldObj.</returns>
		bool MapToMapFromEntity(ISessionImplementor session, IDictionary<string, object> data, object newObj, object oldObj);

		/// <summary>
		/// Maps properties from the given map to the given object.
		/// </summary>
		/// <param name="verCfg">Versions configuration.</param>
		/// <param name="obj">object to map to.</param>
		/// <param name="data">Data to map from.</param>
		/// <param name="primaryKey">Primary key of the object to which we map (for relations)</param>
		/// <param name="versionsReader">VersionsReader for reading relations</param>
		/// <param name="revision">Revision at which the object is read, for reading relations</param>
		void MapToEntityFromMap(AuditConfiguration verCfg, object obj, IDictionary data, object primaryKey,
								IAuditReaderImplementor versionsReader, long revision);

		/// <summary>
		/// Maps collection changes
		/// </summary>
		/// <param name="session">The current session</param>
		/// <param name="referencingPropertyName">Name of the field, which holds the collection in the entity.</param>
		/// <param name="newColl">New collection, after updates.</param>
		/// <param name="oldColl">Old collection, before updates.</param>
		/// <param name="id">Id of the object owning the collection.</param>
		/// <returns>List of changes that need to be performed on the persistent store.</returns>
		IList<PersistentCollectionChangeData> MapCollectionChanges(ISessionImplementor session,
																	string referencingPropertyName,
																  IPersistentCollection newColl,
																  object oldColl, object id);

		void MapModifiedFlagsToMapFromEntity(ISessionImplementor session, IDictionary<string, object> data, object newObj, object oldObj);
		void MapModifiedFlagsToMapForCollectionChange(string collectionPropertyName, IDictionary<string, object> data);
	}
}
