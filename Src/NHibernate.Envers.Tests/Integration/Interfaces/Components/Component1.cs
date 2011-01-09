namespace NHibernate.Envers.Tests.Integration.Interfaces.Components
{
	public class Component1 : IComponent
	{
		public virtual string Data { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as Component1;
			if (casted == null)
				return false;
			return (Data.Equals(casted.Data));
		}

		public override int GetHashCode()
		{
			return Data.GetHashCode();
		}
	}
}