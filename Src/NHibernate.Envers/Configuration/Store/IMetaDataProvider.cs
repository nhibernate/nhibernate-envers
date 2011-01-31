using System.Collections.Generic;

namespace NHibernate.Envers.Configuration.Store
{
	/// <summary>
	/// Knows how to create configuration data
	/// </summary>
	public interface IMetaDataProvider
	{
		IDictionary<System.Type, IEntityMeta> CreateMetaData(Cfg.Configuration nhConfiguration);
	}
}