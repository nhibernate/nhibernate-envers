using System.Collections.Generic;
using System.Text;

namespace NHibernate.Envers.Tools
{
    /**
     * @author Adam Warski (adam at warski dot org)
     */
    public static class StringTools
    {
        /**
         * @param s String, from which to get the last component.
         * @return The last component of the dot-separated string <code>s</code>. For example, for a string
         * "a.b.c", the result is "c".
         */
        public static string GetLastComponent(string s)
        {
            if (s == null)
            {
                return null;
            }

            var lastDot = s.LastIndexOf(".");
            return lastDot == -1 ? s : s.Substring(lastDot + 1);
        }

        /**
         * To the given string builder, appends all strings in the given iterator, separating them with the given
         * separator. For example, for an interator "a" "b" "c" and separator ":" the output is "a:b:c".
         * @param sb String builder, to which to append.
         * @param contents Strings to be appended.
         * @param separator Separator between subsequent content.
         */
        public static void Append(StringBuilder sb, IEnumerator<string> contents, string separator)
        {
            bool isFirst = true;

            contents.Reset();
            while (contents.MoveNext())
            {
                if (!isFirst)
                {
                    sb.Append(separator);
                }

                sb.Append(contents.Current);

                isFirst = false;
            }
        }
    }
}
