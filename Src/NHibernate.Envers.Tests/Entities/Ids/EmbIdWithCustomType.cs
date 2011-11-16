namespace NHibernate.Envers.Tests.Entities.Ids
{
	public class EmbIdWithCustomType
	{
		public virtual int X { get; set; }
		public virtual CustomEnum CustomEnum { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as EmbIdWithCustomType;
			if (casted == null)
				return false;
			return X == casted.X && CustomEnum == casted.CustomEnum;
		}

		public override int GetHashCode()
		{
			return X ^ CustomEnum.GetHashCode();
		}
	}
}