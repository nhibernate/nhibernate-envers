using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation.Component
{
    /**
     * A mapper for reading and writing a property straight to/from maps. This mapper cannot be used with middle tables,
     * but only with "fake" bidirectional indexed relations. 
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public sealed class MiddleStraightComponentMapper : IMiddleComponentMapper {
        private readonly String _propertyName;

        public MiddleStraightComponentMapper(String propertyName) {
            this._propertyName = propertyName;
        }

        public Object MapToObjectFromFullMap(EntityInstantiator entityInstantiator, IDictionary<String, Object> data,
                                             Object dataObject, long revision) {
            if (!data.ContainsKey(_propertyName)) return null;
            return data[_propertyName];
        }

        public void MapToMapFromObject(IDictionary<String, Object> data, Object obj) {
            data.Add(_propertyName, obj);
        }

        public void AddMiddleEqualToQuery(Parameters parameters, String prefix1, String prefix2) {
            throw new NotSupportedException("Cannot use this mapper with a middle table!");
        }
    }
}
