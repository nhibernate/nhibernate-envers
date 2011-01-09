using System;

namespace NHibernate.Envers.Compatibility.Attributes
{
    /**
     * This annotation specifies the version field or property of an entity class that serves as its
     * optimistic lock value. The version is used to ensure integrity when performing the merge
     * operation and for optimistic concurrency control.
     *
     * Only a single Version property or field should be used per class; applications that use more
     * than one Version property or field will not be portable.
     *
     * The Version property should be mapped to the primary table for the entity class; applications
     * that map the Version property to a table other than the primary table will not be portable.
     *
     * The following types are supported for version properties: int, Integer, short, Short, long,
     * Long, Timestamp.
     *  
     * @author Simon Duduica, port of javax.persistence annotation by Emmanuel Bernard
     */
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    class VersionAttribute : Attribute
    {
    }
}
