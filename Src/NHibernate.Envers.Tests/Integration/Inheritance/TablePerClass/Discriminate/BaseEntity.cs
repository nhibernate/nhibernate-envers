using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Integration.Inheritance.TablePerClass.Discriminate
{
	[Audited]
	public class BaseEntity
	{
		public virtual long Id { get; set; }
		public virtual ClassTypeEntity TypeId { get; set; }
		public virtual string Data { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as BaseEntity;
			if(casted==null)
				return false;

			return Id == casted.Id
				&& TypeId.Id == casted.TypeId.Id
				&& string.Equals(Data, casted.Data);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		} 
	}
}