namespace NHibernate.Envers.Tests.NetSpecific.Integration.InsertUpdateFalse
{
	public class ChildComponent
	{
		public virtual int NoUpdateInsert { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as ChildComponent;
			if (casted == null)
				return false;
			return NoUpdateInsert == casted.NoUpdateInsert;
		}

		public override int GetHashCode()
		{
			return NoUpdateInsert;
		}
	}
}