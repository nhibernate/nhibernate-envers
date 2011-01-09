using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NHibernate.Envers.Tools.Graph
{
    /**
     * A graph vertex - stores its representation, neighbours, start and end time in (D|B)FS.
 * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public class Vertex<R> {
        public R Representation { get; private set; }
        public IList<Vertex<R>> Neighbours { get; private set; }

        public int StartTime { get; set; }
        public int EndTime { get; set; }

        public Vertex(R representation) {
            this.Representation = representation;
            this.Neighbours = new List<Vertex<R>>();
            this.StartTime = 0;
            this.EndTime = 0;
        }

        public void addNeighbour(Vertex<R> n) {
            Neighbours.Add(n);
        }
    }
}
