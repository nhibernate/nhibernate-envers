using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Envers.Entities.Mapper.Relation.Component;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
    /**
     * A data holder for a middle relation component (which is either the collection element or index):
     * - component mapper used to map the component to and from versions entities
     * - an index, which specifies in which element of the array returned by the query for reading the collection the data
     * of the component is
     * @author Adam Warski (adam at warski dot org)
     */
    public sealed class MiddleComponentData {
        public IMiddleComponentMapper ComponentMapper { get; private set; }
        public int ComponentIndex { get; private set; }

        public MiddleComponentData(IMiddleComponentMapper componentMapper, int componentIndex) {
            this.ComponentMapper = componentMapper;
            this.ComponentIndex = componentIndex;
        }
    }
}
