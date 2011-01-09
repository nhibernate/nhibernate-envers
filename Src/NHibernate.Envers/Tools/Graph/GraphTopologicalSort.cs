using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers.Tools.Graph
{
    /**
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public class GraphTopologicalSort {
        /**
         * Sorts a graph topologically.
         * @param definer Defines a graph (values and representations) to sort.
         * @return Values of the graph, sorted topologically.
         */
        public static  IList<V> Sort<V, R>(IGraphDefiner<V, R> definer) {
            IEnumerable<V> values = definer.GetValues();
            IDictionary<R, Vertex<R>> vertices = new Dictionary<R, Vertex<R>>();

            // Creating a vertex for each representation
            foreach (V v in values) {
                R rep = definer.GetRepresentation(v);
                vertices.Add(rep, new Vertex<R>(rep));
            }

            // Connecting neighbourhooding vertices
            foreach (V v in values) {
                foreach (V vn in definer.GetNeighbours(v)) {
                    vertices[definer.GetRepresentation(v)].addNeighbour(vertices[definer.GetRepresentation(vn)]);
                }
            }

            // Sorting the representations
            IList<R> sortedReps = new TopologicalSort<R>().sort(vertices.Values);

            // Transforming the sorted representations to sorted values 
            IList<V> sortedValues = new List<V>(sortedReps.Count);
            foreach (R rep in sortedReps) {
                sortedValues.Add(definer.GetValue(rep));
            }

            return sortedValues;
        }
    }
}
