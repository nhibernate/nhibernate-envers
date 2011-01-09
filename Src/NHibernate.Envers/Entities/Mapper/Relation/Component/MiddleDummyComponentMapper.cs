using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Tools.Query;

namespace NHibernate.Envers.Entities.Mapper.Relation.Component
{
    public sealed class MiddleDummyComponentMapper: IMiddleComponentMapper
    {
        public object MapToObjectFromFullMap(EntityInstantiator entityInstantiator, IDictionary<string, object> data, object dataObject, long revision)
        {
            return null;
        }

        public void MapToMapFromObject(IDictionary<string, object> data, object obj)
        {
        }

        public void AddMiddleEqualToQuery(Parameters parameters, string prefix1, string prefix2)
        {
        }
    }
}
