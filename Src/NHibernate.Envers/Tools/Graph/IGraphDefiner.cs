using System.Collections.Generic;

namespace NHibernate.Envers.Tools.Graph
{
    /**
     * Defines a graph, where each vertex has a representation, which identifies uniquely a value.
     * Representations are comparable, values - not.
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public interface IGraphDefiner<V, R>
    {
        R GetRepresentation(V v);
        V GetValue(R r);
        IEnumerable<V> GetNeighbours(V v);
        IEnumerable<V> GetValues();
    }
}
