using System;

namespace NHibernate.Envers.Exceptions
{
    public class AuditException : HibernateException 
	{
		public AuditException(string message) 
						: base(message)
		{
		}

		public AuditException(string message, Exception innerException) 
						: base(message, innerException)
		{
		}

		public AuditException(Exception innerException)
						:base(innerException)
		{
		}
	}
}
