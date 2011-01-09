using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Exceptions;
using NHibernate.Envers.Reader;
using NHibernate.Engine;
using System.Reflection;
using NHibernate.Envers.Configuration;
using NHibernate.Collection;
using NHibernate.Envers.Tools;
using NHibernate.Envers.Tools.Reflection;

namespace NHibernate.Envers.Entities.Mapper
{
    /**
    ///@author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public class ComponentPropertyMapper : IPropertyMapper, ICompositeMapperBuilder {
        private readonly PropertyData propertyData;
        private readonly MultiPropertyMapper _delegate;
	    private readonly String componentClassName;

        public ComponentPropertyMapper(PropertyData propertyData, String componentClassName) {
            this.propertyData = propertyData;
            this._delegate = new MultiPropertyMapper();
		    this.componentClassName = componentClassName;
        }

	    public void Add(PropertyData propertyData) {
            _delegate.Add(propertyData);
        }

        public ICompositeMapperBuilder AddComponent(PropertyData propertyData, String componentClassName) {
            return _delegate.AddComponent(propertyData, componentClassName);
        }

        public void AddComposite(PropertyData propertyData, IPropertyMapper propertyMapper) {
            _delegate.AddComposite(propertyData, propertyMapper);
        }

        public bool MapToMapFromEntity(ISessionImplementor session, IDictionary<String, Object> data, 
                                       Object newObj, Object oldObj) {
            return _delegate.MapToMapFromEntity(session, data, newObj, oldObj);
        }

        public void MapToEntityFromMap(AuditConfiguration verCfg, Object obj, IDictionary<String, Object> data, 
                                       Object primaryKey, IAuditReaderImplementor versionsReader, long revision)
        {
            if (data == null || obj == null) 
			{
                return;
            }

            var setter = ReflectionTools.GetSetter(obj.GetType(), propertyData);

		    // If all properties are null and single, then the component has to be null also.
		    var allNullAndSingle = true;
		    foreach (KeyValuePair<PropertyData, IPropertyMapper> property in _delegate.Properties) 
			{
			    if (data[property.Key.Name] != null || !(property.Value is SinglePropertyMapper)) 
				{
				    allNullAndSingle = false;
				    break;
			    }
		    }

		    // And we don't have to set anything on the object - the default value is null
		    if (!allNullAndSingle) 
			{
			    try 
				{
                    var subObj = Activator.CreateInstance(Toolz.ResolveDotnetType(componentClassName));
                    setter.Set(obj, subObj);

				    _delegate.MapToEntityFromMap(verCfg, subObj, data, primaryKey, versionsReader, revision);
			    } 
				catch (Exception e) 
				{
				    throw new AuditException(e);
			    }
		    }
        }

        public IList<PersistentCollectionChangeData> MapCollectionChanges(String referencingPropertyName,
                                                                                        IPersistentCollection newColl,
                                                                                        Object oldColl,
                                                                                        Object id) {
            return _delegate.MapCollectionChanges(referencingPropertyName, newColl, oldColl, id);
        }

    }
}
