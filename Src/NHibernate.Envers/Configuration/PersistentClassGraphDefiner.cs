using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping;
using NHibernate.Envers.Tools.Graph;
using NHibernate.Envers.Tools;
using System.Collections;

namespace NHibernate.Envers.Configuration
{
    /**
     * Defines a graph, where the vertexes are all persistent classes, and there is an edge from
     * p.c. A to p.c. B iff A is a superclass of B.
     * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
     */
    public class PersistentClassGraphDefiner : IGraphDefiner<PersistentClass, String> {
        private NHibernate.Cfg.Configuration cfg;

        public PersistentClassGraphDefiner(NHibernate.Cfg.Configuration cfg) {
            this.cfg = cfg;
        }

        public String GetRepresentation(PersistentClass pc) {
            return pc.EntityName;
        }

        public PersistentClass GetValue(String entityName) {
            return cfg.GetClassMapping(entityName);
        }

        //@SuppressWarnings({"unchecked"})
        private void AddNeighbours(IList<PersistentClass> neighbours, IEnumerator subclassEnumerator) {
            //IEnumerator enu = subclassIterator.GetEnumerator();
            while (subclassEnumerator.MoveNext()) {
                PersistentClass subclass = (PersistentClass)subclassEnumerator.Current;
                neighbours.Add(subclass);
                AddNeighbours(neighbours, subclass.SubclassIterator.GetEnumerator());
            }
        }

        //@SuppressWarnings({"unchecked"})
        public IList<PersistentClass> GetNeighbours(PersistentClass pc) {
            IList<PersistentClass> neighbours = new List<PersistentClass>();

            AddNeighbours(neighbours, (pc.SubclassIterator.GetEnumerator()));

            return neighbours;
        }

        //@SuppressWarnings({"unchecked"})
        public IEnumerable<PersistentClass> GetValues() {
            return cfg.ClassMappings;
        }
    }
}
