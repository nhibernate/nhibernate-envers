namespace NHibernate.Envers.Tests.NetSpecific.Integration.BidirectionalSameColumn
{
	public class ModelConfigurationShared
	{
		public virtual int Id { get; set; }

		public virtual ModelShared Model { get; set; }
	}
}