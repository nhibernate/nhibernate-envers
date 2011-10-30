using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.Components
{
	[Audited]
	public class UniquePropsNotAuditedEntity
	{
		public UniquePropsNotAuditedEntity()
		{
			Data1 = string.Empty;
			Data2 = string.Empty;
		}

		public virtual long Id { get; set; }
		public virtual string Data1 { get; set; }
		[NotAudited]
		public virtual string Data2 { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as UniquePropsNotAuditedEntity;
			if (casted == null)
				return false;
			return Id == casted.Id &&
			       Data1.Equals(casted.Data1) &&
			       Data2.Equals(casted.Data2);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ Data1.GetHashCode() ^ Data2.GetHashCode();
		} 
	}
}