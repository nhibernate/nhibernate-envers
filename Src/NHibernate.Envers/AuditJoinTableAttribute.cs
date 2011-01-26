using System;

namespace NHibernate.Envers
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
	public class AuditJoinTableAttribute : Attribute
	{
		public AuditJoinTableAttribute()
		{
			Name = string.Empty;
			Schema = string.Empty;
			Catalog = string.Empty;
			InverseJoinColumns = new string[0];
		}

		/// <summary>
		/// Name of the join table. Defaults to a concatenation of the names of the primary table of the entity
		/// owning the association and of the primary table of the entity referenced by the association.
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The schema of the join table. Defaults to the schema of the entity owning the association.
		/// </summary>
		public string Schema { get; set; }

		/// <summary>
		/// The catalog of the join table. Defaults to the catalog of the entity owning the association.
		/// </summary>
		public string Catalog { get; set; }

		/// <summary>
		///  The foreign key columns of the join table which reference the primary table of the entity that does not
		///  own the association (i.e. the inverse side of the association).</summary>
		/// </summary>
		public string[] InverseJoinColumns { get; set; }
	}
}
