namespace NHibernate.Envers.Entities.Mapper
{
    public interface ICompositeMapperBuilder : ISimpleMapperBuilder 
	{
		ICompositeMapperBuilder AddComponent(PropertyData propertyData, string componentClassName);
        void AddComposite(PropertyData propertyData, IPropertyMapper propertyMapper);
    }
}
