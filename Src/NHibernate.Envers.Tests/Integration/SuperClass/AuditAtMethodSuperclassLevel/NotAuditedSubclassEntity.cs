namespace NHibernate.Envers.Tests.Integration.SuperClass.AuditAtMethodSuperclassLevel
{
	public class NotAuditedSubclassEntity : AuditedMethodMappedSuperclass
	{
		public virtual int Id { get; set; }
		public virtual string NotAuditedStr { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as NotAuditedSubclassEntity;
			if (casted == null)
				return false;

			if (!base.Equals(obj))
				return false;

			return Id == casted.Id;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode() ^ Id;
		}
	}
}