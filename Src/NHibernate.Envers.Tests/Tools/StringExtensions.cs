using System.Text.RegularExpressions;

namespace NHibernate.Envers.Tests.Tools
{
	public static class StringExtensions
	{
		public static string ReplaceCaseInsensitive(this string theString, string oldValue, string newValue)
		{
			var regEx = new Regex(oldValue, RegexOptions.IgnoreCase);
			return regEx.Replace(theString, newValue);
		}
	}
}