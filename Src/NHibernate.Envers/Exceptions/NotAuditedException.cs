using System;

namespace NHibernate.Envers.Exceptions
{
	[Serializable]
	public class NotAuditedException : AuditException 
	{
		public NotAuditedException(string entityName, string message)
			: base(message)
		{
			EntityName = entityName;
		}

		public string EntityName { get; private set; }
	}
}
