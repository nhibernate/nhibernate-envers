namespace NHibernate.Envers.Entities
{
    public class PropertyData 
	{
        public string Name { get; private set; }
	    public string BeanName { get; private set; }
        public string AccessType {get; private set;}
        public ModificationStore Store {get; private set;}

		/// <summary>
		/// Copies the given property data, except the name.
		/// </summary>
		/// <param name="newName">New name</param>
		/// <param name="propertyData">Property data to copy the rest of properties from.</param>
        public PropertyData(string newName, PropertyData propertyData) 
		{
            Name = newName;
		    BeanName = propertyData.BeanName;
            AccessType = propertyData.AccessType;
            Store = propertyData.Store;
        }

        /// <summary>
        /// 
        /// </summary>
		/// <param name="name">Name of the property.</param>
		/// <param name="beanName">Name of the property in the bean.</param>
		/// <param name="accessType">Accessor type for this property.</param>
		/// <param name="store">How this property should be stored.</param>
        public PropertyData(string name, string beanName, string accessType, ModificationStore store) 
		{
            Name = name;
		    BeanName = beanName;
            AccessType = accessType;
            Store = store;
        }

	    public override bool Equals(object obj) 
		{
		    if (this == obj) return true;
		    if (obj == null || GetType() != obj.GetType()) return false;

		    var that = (PropertyData) obj;

		    if (AccessType != null ? !AccessType.Equals(that.AccessType) : that.AccessType != null) return false;
		    if (BeanName != null ? !BeanName.Equals(that.BeanName) : that.BeanName != null) return false;
		    if (Name != null ? !Name.Equals(that.Name) : that.Name != null) return false;
		    if (Store != that.Store) return false;

		    return true;
	    }

	    public override int GetHashCode() 
		{
		    var result = Name != null ? Name.GetHashCode() : 0;
		    result = 31 * result + (BeanName != null ? BeanName.GetHashCode() : 0);
		    result = 31 * result + (AccessType != null ? AccessType.GetHashCode() : 0);
		    result = 31 * result + (Store.GetHashCode());
		    return result;
	    }
    }
}
