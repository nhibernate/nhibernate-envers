using NHibernate.Envers.Configuration.Attributes;

namespace NHibernate.Envers.Tests.Entities.Components
{
	[Audited]
	public class Component4
	{
		public virtual string Key { get; set; }
		public virtual string Value { get; set; }
		[NotAudited]
		public virtual string Description { get; set; }

		public override bool Equals(object obj)
		{
			var other = obj as Component4;
			if (other == null)
				return false;

			if (Description != null ? !Description.Equals(other.Description) : other.Description != null) return false;

			if (Key != null ? !Key.Equals(other.Key) : other.Key != null) return false;
			if (Value != null ? !Value.Equals(other.Value) : other.Value != null) return false;

			return true;
		}

		public override int GetHashCode()
		{
			const int prime = 31;
			var result = 1;
			result = prime * result + ((Description == null) ? 0 : Description.GetHashCode());
			result = prime * result + ((Key == null) ? 0 : Key.GetHashCode());
			result = prime * result + ((Value == null) ? 0 : Value.GetHashCode());

			return result;
		}
	}
}