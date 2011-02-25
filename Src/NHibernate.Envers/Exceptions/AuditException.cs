using System;
using System.Runtime.Serialization;

namespace NHibernate.Envers.Exceptions
{
	[Serializable]
	public class AuditException : HibernateException 
	{
		public AuditException()
		{
		}

		public AuditException(string message) 
						: base(message)
		{
		}

		public AuditException(string message, Exception innerException) 
						: base(message, innerException)
		{
		}

		protected AuditException(SerializationInfo info, StreamingContext context) 
						: base(info, context)
	    {
		}
	}
}
