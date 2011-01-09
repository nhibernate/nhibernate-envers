namespace NHibernate.Envers.Tests.Entities.CustomType
{
	public class CompositeCustomTypeEntity 
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual Component Component { get; set; }
	}
}