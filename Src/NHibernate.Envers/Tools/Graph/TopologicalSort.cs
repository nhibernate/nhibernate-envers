using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Envers.Tools.Graph
{
	/**
	 * Topological sorting of a graph - based on DFS.
	 * @author Simon Duduica, port of Envers omonyme class by Adam Warski (adam at warski dot org)
	 */
	public class TopologicalSort<R>
	{
		private List<R> sorted;
		private int time;

		private void process(Vertex<R> v)
		{
			if (v.StartTime != 0)
			{
				// alread processed
				return;
			}

			v.StartTime = time++;

			foreach (var n in v.Neighbours)
			{
				process(n);
			}

			v.EndTime = time++;

			sorted.Add(v.Representation);
		}

		public List<R> Sort(ICollection<Vertex<R>> vertices)
		{
			sorted = new List<R>(vertices.Count);

			time = 1;

			foreach (var v in vertices.Where(v => v.EndTime == 0))
			{
				process(v);
			}

			sorted.Reverse();
			// ORIG: Collections.reverse(sorted);

			return sorted;
		}
	}
}
