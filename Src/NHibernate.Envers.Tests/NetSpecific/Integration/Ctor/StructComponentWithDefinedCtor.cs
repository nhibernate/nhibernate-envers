namespace NHibernate.Envers.Tests.NetSpecific.Integration.Ctor
{
	public struct StructComponentWithDefinedCtor
	{
		private int _value;
		
		public StructComponentWithDefinedCtor(int value)
		{
			_value = value;
		}

		public int Value
		{
			get { return _value; }
			set { _value = value; }
		}
	}
}