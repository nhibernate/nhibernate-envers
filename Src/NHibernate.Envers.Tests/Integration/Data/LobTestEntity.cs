namespace NHibernate.Envers.Tests.Integration.Data
{
	public class LobTestEntity
	{
		public virtual int Id { get; set; }
		[Audited]
		public virtual string StringLob { get; set; }
		[Audited]
		public virtual byte[] ByteLob { get; set; }

		/* not supported outofthe box in nh (?)
		[Audited]
		public virtual char[] CharLob { get; set; }
		*/

		public override bool Equals(object obj)
		{
			var other = obj as LobTestEntity;
			if (other == null)
				return false;
			return other.Id == Id && 
					other.StringLob.Equals(StringLob) && 
					byteBlobEquals(other.ByteLob, ByteLob);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode() ^ 
					StringLob.GetHashCode() ^ 
					ByteLob.GetHashCode();
		}

		private static bool byteBlobEquals(byte[] arr1, byte[] arr2)
		{
			if (arr1.Length != arr2.Length)
				return false;
			for (var i = 0; i < arr1.Length; i++)
				if (arr1[i] != arr2[i])
					return false;
			return true;
		}
	}
}