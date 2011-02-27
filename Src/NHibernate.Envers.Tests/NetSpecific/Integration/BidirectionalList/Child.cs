using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.NetSpecific.Integration.BidirectionalList
{
	[Audited]
	public class Child
	{
		public virtual int Id { get; set; }
		public virtual Parent Parent { get; set; }
		public virtual int IndexOrder
		{
			get { return Parent.Children.IndexOf(this); }
		}

		public override bool Equals(object obj)
		{
			var casted = obj as Child;
			if (casted == null)
				return false;
			return (Id == casted.Id);
		}

		public override int GetHashCode()
		{
			return Id;
		}
	}
}