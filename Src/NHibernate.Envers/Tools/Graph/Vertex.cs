using System.Collections.Generic;

namespace NHibernate.Envers.Tools.Graph
{
	/// <summary>
	/// A graph vertex - stores its representation, neighbours, start and end time in (D|B)FS.
	/// </summary>
	/// <typeparam name="R"></typeparam>
	public class Vertex<R> 
	{
		public Vertex(R representation)
		{
			Representation = representation;
			Neighbours = new List<Vertex<R>>();
			StartTime = 0;
			EndTime = 0;
		}

		public R Representation { get; private set; }
		public ICollection<Vertex<R>> Neighbours { get; private set; }

		public int StartTime { get; set; }
		public int EndTime { get; set; }

		public void AddNeighbour(Vertex<R> n) 
		{
			Neighbours.Add(n);
		}
	}
}
