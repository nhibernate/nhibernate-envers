namespace NHibernate.Envers.Tests.Integration.OneToOne.BiDirectional
{
	public class BiRefEdEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual string Data { get; set; }

		[Audited]
		public virtual BiRefIngEntity Referencing { get; set; }
	}
}