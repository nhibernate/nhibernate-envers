using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.Component
{
	[Audited]
	public class ComponentOwner
	{
		public virtual int Id { get; set; }
		public virtual StructComponent Component { get; set; }


		public override bool Equals(object obj)
		{
			var casted = obj as ComponentOwner;
			if (casted == null)
				return false;
			return (Id == casted.Id && Component.Equals(casted.Component));
		}

		public override int GetHashCode()
		{
			return Id ^Component.GetHashCode();
		}
	}
}