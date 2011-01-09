using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Entities.Mapper;
using NHibernate.Mapping;
using NHibernate.Envers.Configuration.Metadata.Reader;
using NHibernate.Envers.Entities;
using NHibernate.Envers.Configuration.Metadata;
using NHibernate.Envers.Tools.Graph;
using NHibernate.Properties;
using NHibernate.Envers.Tools;
using NHibernate.Envers.Entities.Mapper.Id;
using System.Threading;
using System.Reflection;
using NHibernate.Envers.Exceptions;
using NHibernate.Util;



namespace NHibernate.Envers.Entities.Mapper.Id
{
/**
 * @author Catalina Panait, port of Envers omonyme class by Adam Warski (adam at warski dot org)
 */
    public abstract class  AbstractCompositeIdMapper : AbstractIdMapper , ISimpleIdMapperBuilder
    {
        protected IDictionary<PropertyData, SingleIdMapper> ids;
        protected System.Type compositeIdClass;

        protected AbstractCompositeIdMapper(System.Type compositeIdClass)
        {
            //Simon 27/06/2010 - era LinkedHashMap
            ids = new Dictionary<PropertyData, SingleIdMapper>();

            this.compositeIdClass = compositeIdClass;
        }

        public void Add(PropertyData propertyData)
        {
            ids.Add(propertyData, new SingleIdMapper(propertyData));     
        }

        public override Object MapToIdFromMap(IDictionary<String, Object> data) //(Map data)
        {//<String, Object>  sau <PropertyData, SingleIdMapper>
        Object ret;
        try {
            ret = Activator.CreateInstance(compositeIdClass);
            //ret = Thread.currentThread().getContextClassLoader().loadClass(compositeIdClass).newInstance();
        } catch (Exception e) {
            throw new AuditException(e);
        }

        foreach (SingleIdMapper mapper in ids.Values) {
            mapper.MapToEntityFromMap(ret, data);
        }

        return ret;
    }
    }
}
