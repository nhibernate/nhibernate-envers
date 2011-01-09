using System;

namespace NHibernate.Envers.Exceptions
{
    public class AuditException : HibernateException 
	{
		public AuditException(String message) 
						: base(message)
		{
		}

		public AuditException(String message, System.Exception innerException) 
						: base(message, innerException)
		{
		}

		public AuditException(System.Exception innerException)
						:base(innerException)
		{
		}
	}
}
