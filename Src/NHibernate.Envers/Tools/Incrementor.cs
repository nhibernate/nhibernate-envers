namespace NHibernate.Envers.Tools
{
	/// <summary>
	/// Util class to get the incremental number for aliases and parameters
	/// </summary>
	/// <remarks>
	/// Increment the value at each get.
	/// </remarks>
	public class Incrementor
	{
		private int value;

		public int Get()
		{
			return value++;
		}
	}
}