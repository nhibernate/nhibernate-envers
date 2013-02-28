using System.Collections.Generic;

namespace NHibernate.Envers.Entities.Mapper
{
	public interface ICompositeMapperBuilder : ISimpleMapperBuilder 
	{
		ICompositeMapperBuilder AddComponent(PropertyData propertyData, string componentClassName);
		void AddComposite(PropertyData propertyData, IPropertyMapper propertyMapper);
		IDictionary<PropertyData, IPropertyMapper> Properties { get; }
	}
}
