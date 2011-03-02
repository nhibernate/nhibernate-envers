using System.Collections.Generic;
using NHibernate.Envers.Configuration.Fluent;
using NHibernate.Envers.Configuration.Store;

namespace NHibernate.Envers.Tests.Tools
{
	public static class FluentExtensions
	{
		public static IDictionary<System.Type, IEntityMeta> BuildMetaData(this FluentConfiguration fluentConfiguration)
		{
			return ((IMetaDataProvider) fluentConfiguration).CreateMetaData(null);
		}
	}
}