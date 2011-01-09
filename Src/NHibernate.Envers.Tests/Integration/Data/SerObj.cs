using System;

namespace NHibernate.Envers.Tests.Integration.Data
{
	[Serializable]
	public class SerObj
	{
		public virtual string Data { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as SerObj;
			if (other == null)
				return false;
			return other.Data.Equals(Data);
		}

		public override int GetHashCode()
		{
			return Data.GetHashCode();
		}
	}
}