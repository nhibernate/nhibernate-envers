using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Inheritance.Single.DiscriminatorFormula
{
	[Audited]
	public class ParentEntity
	{
		public virtual long Id { get; set; }
		public virtual long TypeId { get; set; }
		public virtual string Data { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as ParentEntity;
			if(casted==null)
				return false;
			return Id == casted.Id && TypeId == casted.TypeId && Data.Equals(casted.Data);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ TypeId.GetHashCode() ^Data.GetHashCode();
		} 
	}
}