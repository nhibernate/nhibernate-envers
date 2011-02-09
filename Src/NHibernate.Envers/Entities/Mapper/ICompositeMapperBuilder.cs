using System;

namespace NHibernate.Envers.Entities.Mapper
{
    public interface ICompositeMapperBuilder : ISimpleMapperBuilder 
	{    
        ICompositeMapperBuilder AddComponent(PropertyData propertyData, String componentClassName);
        void AddComposite(PropertyData propertyData, IPropertyMapper propertyMapper);
    }
}
