using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Naming.Quotation
{
	public class QuotedFieldsEntity
	{
		public virtual long Id { get; set; }
		[Audited]
		public virtual string Data1 { get; set; }
		[Audited]
		public virtual int Data2 { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as QuotedFieldsEntity;
			if (casted == null)
				return false;
			if (Data1 != null ? !Data1.Equals(casted.Data1) : casted.Data1 != null) 
				return false;
			return Id == casted.Id && Data2.Equals(casted.Data2);
		}

		public override int GetHashCode()
		{
			var strHash = Data1 != null ? Data1.GetHashCode() : 0;
			return Id.GetHashCode() ^ strHash.GetHashCode() ^ Data2;
		}
	}
}