namespace NHibernate.Envers.Tests.Entities.CustomType
{
	public class ParametrizedCustomTypeEntity
	{
		public virtual int Id { get; set; }
		[Audited]
		public virtual string Str { get; set; }
	}
}