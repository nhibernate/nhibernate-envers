using NHibernate.Envers.Configuration.Store;

namespace NHibernate.Envers.Configuration.Fluent
{
	public interface IMainConfiguration
	{
		IMetaDataProvider BuildMetaDataProvider();
	}
}