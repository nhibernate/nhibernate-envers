using System;

namespace NHibernate.Envers.Tests.Entities.Ids
{
	[Serializable]
	public class MulId
	{
		public virtual int Id1 { get; set; }
		public virtual int Id2 { get; set; }

		public override bool Equals(object obj)
		{
			var casted = obj as EmbId;
			if (casted == null)
				return false;
			return (Id1 == casted.X && Id2 == casted.Y);
		}

		public override int GetHashCode()
		{
			return Id1 ^ Id2;
		}
	}
}