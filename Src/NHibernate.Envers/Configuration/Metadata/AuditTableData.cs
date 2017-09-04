namespace NHibernate.Envers.Configuration.Metadata
{
	public class AuditTableData 
	{
		public AuditTableData(string auditEntityName, string auditTableName, string schema, string catalog) 
		{
			AuditEntityName = auditEntityName;
			AuditTableName = auditTableName;
			Schema = schema;
			Catalog = catalog;
		}

		public string AuditEntityName { get; }
		public string AuditTableName { get; }
		public string Schema { get; }
		public string Catalog { get; }
	}
}
