namespace NHibernate.Envers.Tests.Entities.Components
{
	public class DefaultValueComponent1
	{
		public virtual string Str1 { get; set; }
		public DefaultValueComponent2 Comp2 = new DefaultValueComponent2();

		public override bool Equals(object obj)
		{
			var casted = obj as DefaultValueComponent1;
			if (casted == null)
				return false;
			if (Str1 != null ? !Str1.Equals(casted.Str1) : casted.Str1 != null)
				return false;
			if (Comp2 != null ? !Comp2.Equals(casted.Comp2) : casted.Comp2 != null)
				return false;
			return true;
		}

		public override int GetHashCode()
		{
			var str1Hash = Str1 != null ? Str1.GetHashCode() : 0;
			var compHas = Comp2 != null ? Comp2.GetHashCode() : 0;
			return str1Hash ^ compHas;

		}
	}
}