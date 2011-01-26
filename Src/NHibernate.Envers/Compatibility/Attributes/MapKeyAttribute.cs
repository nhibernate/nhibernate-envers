using System;

namespace NHibernate.Envers.Compatibility.Attributes
{
    //rk remove this one!
    /**
     * Is used to specify the map key for associations of type Map.
     * If a persistent field or property other than the primary key is used as a map key then it
     * is expected to have a uniqueness constraint associated with it.
     *
     * @author Simon Duduica, port of javax.persistence annotation by Emmanuel Bernard
     */
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class MapKeyAttribute: Attribute 
    {
	    /**
	     * The name of the persistent field or property of the associated entity that is used as the map key.
	     * If the name element is not specified, the primary key of the associated entity is used as the map key.
	     * If the primary key is a composite primary key and is mapped as IdClass, an instance of the primary key
	     * class is used as the key.
	     */
	    public string Name = string.Empty;
    }
}
