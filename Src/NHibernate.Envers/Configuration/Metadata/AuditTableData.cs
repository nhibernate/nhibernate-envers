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

		public string AuditEntityName { get; private set; }
		public string AuditTableName { get; private set; }
		public string Schema { get; private set; }
		public string Catalog { get; private set; }
	}
}
