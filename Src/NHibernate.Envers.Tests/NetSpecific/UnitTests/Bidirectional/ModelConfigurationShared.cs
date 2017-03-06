namespace NHibernate.Envers.Tests.NetSpecific.UnitTests.Bidirectional
{
	public class ModelConfigurationShared
	{
		public virtual int Id { get; set; }

		public virtual ModelShared Model { get; set; }
	}
}