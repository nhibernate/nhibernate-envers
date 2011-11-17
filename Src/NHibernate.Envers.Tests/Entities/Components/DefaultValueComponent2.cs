namespace NHibernate.Envers.Tests.Entities.Components
{
	public class DefaultValueComponent2
	{
		public string Str1 = "defaultValue";
		public string Str2;

		public override bool Equals(object obj)
		{
			var casted = obj as DefaultValueComponent2;
			if (casted == null)
				return false;
			if (Str1 != null ? !Str1.Equals(casted.Str1) : casted.Str1 != null)
				return false;
			if (Str2 != null ? !Str2.Equals(casted.Str2) : casted.Str2 != null)
				return false;

			return true;
		}

		public override int GetHashCode()
		{
			var str1Hash = Str1 != null ? Str1.GetHashCode() : 0;
			var str2Hash = Str2 != null ? Str2.GetHashCode() : 0;
			return str1Hash ^ str2Hash;
		}
	}
}