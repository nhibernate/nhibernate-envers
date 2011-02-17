namespace NHibernate.Envers.Tests.NetSpecific.Integration.Component
{
	public struct StructComponent2
	{
		public int Data2 { get; set; }

		public override bool Equals(object obj)
		{
			var casted = (StructComponent2)obj;
			return (Data2.Equals(casted.Data2));
		}

		public override int GetHashCode()
		{
			return Data2.GetHashCode();
		}
	}
}