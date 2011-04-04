using System;

namespace NHibernate.Envers.Exceptions
{
	[Serializable]
	public class RevisionDoesNotExistException : AuditException 
	{
		public RevisionDoesNotExistException(long revision)
			: base("Revision " + revision + " does not exist.")
		{
			Revision = revision;
		}

		public RevisionDoesNotExistException(DateTime date)
			: base("There is no revision before or at " + date + ".")
		{
			DateTime = date;
		}

		public long Revision { get; private set; }
		public DateTime DateTime { get; private set; }
	}
}
