using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.InsertUpdateFalse
{
	[Audited]
	public class ParentEntity
	{
		public virtual int Id { get; set; }
		public virtual ChildComponent Component { get; set; }
		public virtual ChildComponent ComponentSetter { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as ParentEntity;
			if (casted == null)
				return false;
			return Id == casted.Id &&
			       Component.Equals(casted.Component) &&
			       ComponentSetter.Equals(casted.ComponentSetter);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}
	}
}