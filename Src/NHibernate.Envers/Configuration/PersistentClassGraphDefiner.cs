using System.Collections.Generic;
using NHibernate.Envers.Tools.Graph;
using NHibernate.Mapping;

namespace NHibernate.Envers.Configuration
{
	/// <summary>
	/// Defines a graph, where the vertexes are all persistent classes, and there is an edge from
	/// p.c. A to p.c. B iff A is a superclass of B.
	/// </summary>
	public class PersistentClassGraphDefiner : IGraphDefiner<PersistentClass, string> 
	{
		private readonly Cfg.Configuration cfg;

		public PersistentClassGraphDefiner(Cfg.Configuration cfg) 
		{
			this.cfg = cfg;
		}

		public string GetRepresentation(PersistentClass pc) 
		{
			return pc.EntityName;
		}

		public PersistentClass GetValue(string entityName) 
		{
			return cfg.GetClassMapping(entityName);
		}

		public IEnumerable<PersistentClass> GetNeighbours(PersistentClass pc) 
		{
			var neighbours = new List<PersistentClass>();
			addNeighbours(neighbours, pc.SubclassIterator);
			return neighbours;
		}

		public IEnumerable<PersistentClass> GetValues() 
		{
			return cfg.ClassMappings;
		}

		private static void addNeighbours(ICollection<PersistentClass> neighbours, IEnumerable<Subclass> subclasses)
		{
			foreach (var subclass in subclasses)
			{
				neighbours.Add(subclass);
				addNeighbours(neighbours, subclass.SubclassIterator);
			}
		}
	}
}
