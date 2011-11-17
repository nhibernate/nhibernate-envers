namespace NHibernate.Envers.Tests.Integration.Inheritance.Mixed
{
	public interface IActivity
	{
		ActivityId Id { get; }
		int SequenceNumber { get; }
	}
}