namespace NHibernate.Envers.Tools
{
	public static class StringTools
	{
		/// <summary>
		/// </summary>
		/// <param name="s">String, from which to get the last component.</param>
		/// <returns>
		/// The last component of the dot-separated string <code>s</code>. For example, for a string
		///	* "a.b.c", the result is "c".
		/// </returns>
		public static string GetLastComponent(string s)
		{
			if (s == null)
			{
				return null;
			}

			var lastDot = s.LastIndexOf(".");
			return lastDot == -1 ? s : s.Substring(lastDot + 1);
		}
	}
}
