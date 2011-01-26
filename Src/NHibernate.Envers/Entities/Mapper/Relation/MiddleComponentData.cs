using NHibernate.Envers.Entities.Mapper.Relation.Component;

namespace NHibernate.Envers.Entities.Mapper.Relation
{
	/// <summary>
	/// A data holder for a middle relation component (which is either the collection element or index):
	/// - component mapper used to map the component to and from versions entities
	/// - an index, which specifies in which element of the array returned by the query for reading the collection the data
	/// of the component is
	/// </summary>
	public sealed class MiddleComponentData 
	{
		public MiddleComponentData(IMiddleComponentMapper componentMapper, int componentIndex) 
		{
			ComponentMapper = componentMapper;
			ComponentIndex = componentIndex;
		}

		public IMiddleComponentMapper ComponentMapper { get; private set; }
		public int ComponentIndex { get; private set; }
	}
}
