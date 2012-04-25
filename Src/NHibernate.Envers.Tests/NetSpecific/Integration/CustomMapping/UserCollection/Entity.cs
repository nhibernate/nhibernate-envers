using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.CustomMapping.UserCollection
{
	[Audited]
	public class Entity
	{
		public Entity()
		{
			SpecialCollection = new SpecialCollection(10);
		}
		public virtual int Id { get; set; }
		[CustomCollection(typeof(CustomCollectionFactory))]
		public virtual ISpecialCollection SpecialCollection { get; set; }
	}
}