using System.Collections.Generic;
using System.Text;

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

		/// <summary>
		/// To the given string builder, appends all strings in the given iterator, separating them with the given
		/// separator. For example, for an interator "a" "b" "c" and separator ":" the output is "a:b:c".
		/// </summary>
		/// <param name="sb">String builder, to which to append.</param>
		/// <param name="contents">Strings to be appended.</param>
		/// <param name="separator">Separator between subsequent content.</param>
		public static void Append(StringBuilder sb, IEnumerable<string> contents, string separator)
		{
			var isFirst = true;
			foreach (var content in contents)
			{
				if (!isFirst)
				{
					sb.Append(separator);
				}

				sb.Append(content);

				isFirst = false;
			}
		}
	}
}
