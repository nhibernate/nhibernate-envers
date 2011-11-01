using System.Collections.Generic;
using System.Linq;

namespace NHibernate.Envers.Tools.Graph
{
	public static class GraphTopologicalSort
	{
		/// <summary>
		/// Sorts a graph topologically.
		/// </summary>
		/// <typeparam name="V"></typeparam>
		/// <typeparam name="R"></typeparam>
		/// <param name="definer">Defines a graph (values and representations) to sort.</param>
		/// <returns>Values of the graph, sorted topologically.</returns>
		public static IList<V> Sort<V, R>(IGraphDefiner<V, R> definer)
		{
			var values = definer.GetValues();

			// Creating a vertex for each representation
			var vertices = values.Select(definer.GetRepresentation)
				.ToDictionary(rep => rep, rep => new Vertex<R>(rep));

			// Connecting neighbourhooding vertices
			foreach (var v in values)
			{
				foreach (var vn in definer.GetNeighbours(v))
				{
					vertices[definer.GetRepresentation(v)].AddNeighbour(vertices[definer.GetRepresentation(vn)]);
				}
			}

			// Sorting the representations
			var sortedReps = new TopologicalSort<R>().Sort(vertices.Values);

			// Transforming the sorted representations to sorted values 
			var sortedValues = new List<V>(sortedReps.Count);
			sortedValues.AddRange(sortedReps.Select(definer.GetValue));

			return sortedValues;
		}
	}
}
