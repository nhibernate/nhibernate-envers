using NHibernate.Envers.Configuration.Attributes;
using NHibernate.Envers.Tests.Entities.Ids;

namespace NHibernate.Envers.Tests.Integration.OneToOne.UniDirectional
{
	public class UniRefIngMulIdEntity
	{
		public virtual int Id { get; set; }

		[Audited]
		public virtual string Data { get; set; }

		[Audited]
		public virtual EmbIdTestEntity Reference { get; set; }
	}
}