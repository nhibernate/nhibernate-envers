using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping;

namespace NHibernate.Envers.Tools
{
    /**
     * @author Adam Warski (adam at warski dot org)
     */
    public class MappingTools
    {
        /**
         * @param componentName Name of the component, that is, name of the property in the entity that references the
         * component.
         * @return A prefix for properties in the given component.
         */
        public static String createComponentPrefix(String componentName)
        {
            return componentName + "_";
        }

        /**
         * @param referencePropertyName The name of the property that holds the relation to the entity.
         * @return A prefix which should be used to prefix an id mapper for the related entity.
         */
        public static String createToOneRelationPrefix(String referencePropertyName)
        {
            return referencePropertyName + "_";
        }

        public static String getReferencedEntityName(IValue value) {
            if (value is ToOne) {
                return ((ToOne) value).ReferencedEntityName;
            } else if (value is OneToMany) {
                return ((OneToMany) value).ReferencedEntityName;
            } else if (value is NHibernate.Mapping.Collection) {
                return getReferencedEntityName(((NHibernate.Mapping.Collection)value).Element);
            }

            return null;
        }
    }
}
