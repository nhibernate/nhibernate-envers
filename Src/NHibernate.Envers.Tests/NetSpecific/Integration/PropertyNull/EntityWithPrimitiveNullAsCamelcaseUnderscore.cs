using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.PropertyNull
{
	[Audited]
	public class EntityWithPrimitiveNullAsCamelcaseUnderscore
	{
		private int? _value;

		public virtual int Id { get; set; }
		public virtual int? Value
		{
			get { return _value; }
			set { _value = value; }
		}
	}
}