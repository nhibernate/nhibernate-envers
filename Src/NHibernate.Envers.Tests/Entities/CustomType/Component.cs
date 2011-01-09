using System;

namespace NHibernate.Envers.Tests.Entities.CustomType
{
	public class Component
	{
		public virtual string Prop1 { get; set; }
		public virtual int Prop2 { get; set; }

		public override bool Equals(object o)
		{
			if (this == o) return true;
			if (!(o is Component)) return false;

			var that = (Component) o;

			if (Prop2 != that.Prop2) return false;
			if (Prop1 != null ? !Prop1.Equals(that.Prop1) : that.Prop1 != null) return false;

			return true;
		}

		public override int GetHashCode()
		{
			var result = (Prop1 != null ? Prop1.GetHashCode() : 0);
			result = 31 * result + Prop2;
			return result;
		}
	}
}