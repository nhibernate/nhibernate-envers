using System.Collections;
using System.Collections.Generic;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation.Component
{
    class MiddleRelatedComponentMapper : IMiddleComponentMapper
    {
        private readonly MiddleIdData relatedIdData;

        public MiddleRelatedComponentMapper(MiddleIdData relatedIdData) 
		{
            this.relatedIdData = relatedIdData;
        }

        public object MapToObjectFromFullMap(EntityInstantiator entityInstantiator, 
											IDictionary data,
                                            object dataObject, 
											long revision) 
		{
            return entityInstantiator.CreateInstanceFromVersionsEntity(relatedIdData.EntityName, data, revision);
        }

		public void MapToMapFromObject(IDictionary<string, object> data, object obj) 
		{
            relatedIdData.PrefixedMapper.MapToMapFromEntity(data, obj);
        }

		public void AddMiddleEqualToQuery(Parameters parameters, string prefix1, string prefix2) 
		{
            relatedIdData.PrefixedMapper.AddIdsEqualToQuery(parameters, prefix1, prefix2);
        }
    }
}
