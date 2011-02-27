using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Access
{
	[Audited]
	public class ReadonlyEntity
	{
		private int _readonlyData;
		public virtual int Id { get; set; }
		public virtual int Data { get; set; }
		public virtual int ReadonlyData 
		{
			get
			{
				return _readonlyData;
			}
		}

		public virtual void SetReadOnlyData(int value)
		{
			_readonlyData = value;
		}
	}
}