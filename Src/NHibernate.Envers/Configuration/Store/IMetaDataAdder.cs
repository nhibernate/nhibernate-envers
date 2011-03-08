using System.Collections.Generic;

namespace NHibernate.Envers.Configuration.Store
{
	/// <summary>
	/// Runs after <see cref="IMetaDataProvider"/> has created entity metas.
	/// </summary>
	public interface IMetaDataAdder
	{
		/// <summary>
		/// Adds metadata to <paramref name="currentMetaData"/>
		/// </summary>
		/// <param name="currentMetaData">The orginal meta data</param>
		void AddMetaDataTo(IDictionary<System.Type, IEntityMeta> currentMetaData);
	}
}