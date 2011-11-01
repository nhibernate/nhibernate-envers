using System.Collections.Generic;

namespace NHibernate.Envers.Configuration.Store
{
	/// <summary>
	/// Knows how to create configuration data
	/// </summary>
	public interface IMetaDataProvider
	{
		/// <summary>
		/// Creates the meta data. Normally not used by user.
		/// </summary>
		/// <param name="nhConfiguration">The NH Configuration.</param>
		/// <returns>
		/// A dictionary of <see cref="IEntityMeta"/>, keyed by entity type
		/// </returns>
		IDictionary<System.Type, IEntityMeta> CreateMetaData(Cfg.Configuration nhConfiguration);
	}
}