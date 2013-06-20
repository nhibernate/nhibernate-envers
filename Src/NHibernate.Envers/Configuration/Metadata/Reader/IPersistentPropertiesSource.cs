using System.Collections.Generic;
using NHibernate.Mapping;

namespace NHibernate.Envers.Configuration.Metadata.Reader
{
	public interface IPersistentPropertiesSource
	{
		IEnumerable<DeclaredPersistentProperty> DeclaredPersistentProperties { get; }
		Property VersionedProperty { get; }
		System.Type Class { get; }
		bool IsComponent { get; }
	}
}
