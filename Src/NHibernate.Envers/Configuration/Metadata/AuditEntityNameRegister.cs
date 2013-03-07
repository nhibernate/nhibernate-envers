using System.Collections.Generic;

namespace NHibernate.Envers.Configuration.Metadata
{
	/// <summary>
	/// A register of all audit entity names used so far.
	/// </summary>
	public class AuditEntityNameRegister 
	{
		private readonly ISet<string> auditEntityNames = new HashSet<string>();

		/// <summary>
		/// </summary>
		/// <param name="auditEntityName">Name of the audit entity.</param>
		/// <returns>True if the given audit entity name is already used.</returns>
		private bool check(string auditEntityName) 
		{
			return auditEntityNames.Contains(auditEntityName);
		}

		/// <summary>
		/// Register an audit entity name. If the name is already registered, an exception is thrown.
		/// </summary>
		/// <param name="auditEntityName">
		/// Name of the audit entity.
		/// </param>
		public void Register(string auditEntityName) 
		{
			if (auditEntityNames.Contains(auditEntityName)) 
			{
				throw new MappingException("The audit entity name '" + auditEntityName + "' is already registered.");
			}
			
			auditEntityNames.Add(auditEntityName);
		}

		/// <summary>
		/// Creates a unique (not yet registered) audit entity name by appending consecutive numbers to the base
		/// name. If the base name is not yet used, it is returned unmodified.
		/// </summary>
		/// <param name="baseAuditEntityName">The base entity name.</param>
		/// <returns></returns>
		public string CreateUnique(string baseAuditEntityName) 
		{
			var auditEntityName = baseAuditEntityName;
			var count = 1;
			while (check(auditEntityName)) 
			{
				auditEntityName = baseAuditEntityName + count++;
			}

			return auditEntityName;
		}
	}
}
