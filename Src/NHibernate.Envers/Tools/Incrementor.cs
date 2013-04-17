using System;

namespace NHibernate.Envers.Tools
{
	/// <summary>
	/// Util class to get the incremental number for aliases and parameters
	/// </summary>
	/// <remarks>
	/// Increment the value at each get.
	/// </remarks>
	public class Incrementor : ICloneable
	{
		private int value;

		public int Get()
		{
			return value++;
		}

		public object Clone()
		{
			return new Incrementor {value = value};
		}
	}
}