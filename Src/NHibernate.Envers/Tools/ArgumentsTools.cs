using System;

namespace NHibernate.Envers.Tools
{
	public static class ArgumentsTools
	{
		public static void CheckNotNull(object o, string paramName)
		{
			if (o == null)
			{
				throw new ArgumentNullException(paramName, paramName + " cannot be null.");
			}
		}

		public static void CheckPositive(long i, string paramName)
		{
			if (i <= 0)
			{
				throw new ArgumentOutOfRangeException(paramName, paramName + " has to be greater than 0.");
			}
		}
	}
}
