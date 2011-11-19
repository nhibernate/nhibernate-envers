using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Properties
{
	[Audited]
	public class UnversionedOptimisticLockingFieldEntity
	{
		public virtual int Id { get; set; }
		public virtual string Str { get; set; }
		public virtual int OptLocking { get; set; }

		public override bool Equals(object obj)
		{
			var that = obj as UnversionedOptimisticLockingFieldEntity;
			if (that == null)
				return false;
			if (Str != null ? !Str.Equals(that.Str) : that.Str != null)
				return false;
			return Id == that.Id && OptLocking == that.OptLocking;
		}

		public override int GetHashCode()
		{
			var strHash = Str != null ? Str.GetHashCode() : 0;
			return Id ^ strHash ^ OptLocking;
		} 
	}
}