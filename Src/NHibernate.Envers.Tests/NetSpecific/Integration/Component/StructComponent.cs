namespace NHibernate.Envers.Tests.NetSpecific.Integration.Component
{
	public struct StructComponent
	{
		public string Data1 { get; set; }
		public StructComponent2 NestedComponent { get; set; }

		public override bool Equals(object obj)
		{
			var casted = (StructComponent)obj;
			if (Data1 == null && casted.Data1 == null)
				return true;
			if (Data1 == null || casted.Data1 == null)
				return false;
			return (Data1.Equals(casted.Data1) && NestedComponent.Equals(casted.NestedComponent));
		}

		public override int GetHashCode()
		{
			if (Data1 == null)
				return NestedComponent.GetHashCode();
			return Data1.GetHashCode() ^ NestedComponent.GetHashCode();
		}
	}
}