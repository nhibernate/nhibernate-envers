namespace NHibernate.Envers.Tests.Integration.OneToOne.BiDirectional
{
	public class BiRefIngEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual string Data { get; set; }

		[Audited]
		public virtual BiRefEdEntity Reference { get; set; }
	}
}