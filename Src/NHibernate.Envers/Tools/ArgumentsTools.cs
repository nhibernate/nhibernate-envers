using System;

namespace NHibernate.Envers.Tools
{
    public static class ArgumentsTools
    {
        public static void CheckNotNull(Object o, String paramName)
        {
            if (o == null)
            {
                throw new ArgumentNullException(paramName + " cannot be null.");
            }
        }

        public static void CheckPositive(long i, String paramName)
        {
            if (i <= 0)
            {
                throw new ArgumentOutOfRangeException(paramName + " has to be greater than 0.");
            }
        }
    }
}
