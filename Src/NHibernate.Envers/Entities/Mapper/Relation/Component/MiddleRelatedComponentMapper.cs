using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation.Component
{
    /**
     * @author Simon Duduica, port of omonyme class by Adam Warski (adam at warski dot org)
     */
    class MiddleRelatedComponentMapper : IMiddleComponentMapper
    {
        private readonly MiddleIdData relatedIdData;

        public MiddleRelatedComponentMapper(MiddleIdData relatedIdData) {
            this.relatedIdData = relatedIdData;
        }

        public object MapToObjectFromFullMap(EntityInstantiator entityInstantiator, 
											IDictionary<String, Object> data,
                                            object dataObject, 
											long revision) 
		{
            return entityInstantiator.CreateInstanceFromVersionsEntity(relatedIdData.EntityName, data, revision);
        }

        public void MapToMapFromObject(IDictionary<String, Object> data, Object obj) {
            relatedIdData.PrefixedMapper.MapToMapFromEntity(data, obj);
        }

        public void AddMiddleEqualToQuery(Parameters parameters, String prefix1, String prefix2) {
            relatedIdData.PrefixedMapper.AddIdsEqualToQuery(parameters, prefix1, prefix2);
        }
    }
}
