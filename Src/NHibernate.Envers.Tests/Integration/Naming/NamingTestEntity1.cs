namespace NHibernate.Envers.Tests.Integration.Naming
{
	[AuditTable("naming_test_entity_1_versions")]
	public class NamingTestEntity1
	{
		public virtual int Id { get; set; }
		[Audited]
		public virtual string Data { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as NamingTestEntity1;
			if (casted == null)
				return false;
			return (Id == casted.Id && Data.Equals(casted.Data));
		}

		public override int GetHashCode()
		{
			return Id ^ Data.GetHashCode();
		}
	}
}